using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class CuentasControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task CrearUsuario()
        {
            var nombreDB = Guid.NewGuid().ToString();
            await CrearUsuarioHelper(nombreDB);
            var contexto2 = ConstruirContext(nombreDB);
            var conteo = await contexto2.Users.CountAsync();
            Assert.AreEqual(1, conteo);
        }

        private async Task CrearUsuarioHelper(string nombreDB)
        {
            var cuentasController = ConstruirCuentasController(nombreDB);
            var userInfo = new UserInfo() { Email = "ejemplo@hotmail.com", Password = "aA123456!" };
            await cuentasController.CreateUser(userInfo);
        }

        [TestMethod]
        public async Task UsuarioNoPuedeLogearse()
        {
            var nombreDB = Guid.NewGuid().ToString();
            await CrearUsuarioHelper(nombreDB);
            var controller = ConstruirCuentasController(nombreDB);
            var userInfo = new UserInfo() { Email = "ejemplo@hotmail.com", Password = "malPassword" };

            var respuesta = await controller.Login(userInfo);
            Assert.IsNull(respuesta.Value);

            var resultado = respuesta.Result as BadRequestObjectResult;
            Assert.IsNotNull(resultado);    
        }

        [TestMethod]
        public async Task UsuarioPuedeLogearse()
        {
            var nombreDB = Guid.NewGuid().ToString();
            await CrearUsuarioHelper(nombreDB);
            var controller = ConstruirCuentasController(nombreDB);
            var userInfo = new UserInfo() { Email = "ejemplo@hotmail.com", Password = "aA123456!" };

            var respuesta = await controller.Login(userInfo);
            Assert.IsNotNull(respuesta.Value);
            Assert.IsNotNull(respuesta.Value.Token);
        }

        // <--------------------------------------------------- MÉTODOS AUXILIARES ------------------------------------------------>

        // Creamos el método auxiliar para crear el controlador de cuentas, para tener todo centralizado.
        private CuentasController ConstruirCuentasController(string nombreDB)
        {
            var context = ConstruirContext(nombreDB);
            var miUserStore = new UserStore<IdentityUser>(context);     
            var userManager = BuildUserManager(miUserStore);
            var mapper = ConfigurarAutoMapper();

            var httpContext = new DefaultHttpContext();     // Instanciamos un httpContext para poder realizar un mock de IAuthenticationService,
                                                            // para que la autenticación no sea 100% real.
            MockAuth(httpContext);

            var signInManager = SetupSignInManager(userManager, httpContext);

            var miConfiguracion = new Dictionary<string, string>
            {
                {"JWT:key", "ASDKMASKJDASKDANSLDANSKAJSDALSCNALSDJALASDJALSD" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(miConfiguracion)
                .Build();

            return new CuentasController(userManager, signInManager, configuration, context, mapper);
        }


        // NECESITAMOS ESTAS 2 INSTANCIACIONES DE CLASES PARA PODER UTILIZARLAS, DEBIDO A QUE AMBAS EN
        // NUESTRO CÓDIGO SON CLASES CONCRETAS, Y NO INTERFACES DE LAS CUALES PODAMOS HACER UN MOCK
        // Estas clases nos proveen de un UserManager, y un SignInManager, los cuales necesitamos.
         
        private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;

            options.Setup(o => o.Value).Returns(idOptions);

            var userValidators = new List<IUserValidator<TUser>>();

            var validator = new Mock<IUserValidator<TUser>>();              // AMBAS LAS SACAMOS DEL CÓDIGO FUENTE DE .NET CORE 6
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());

            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return userManager;
        }

        private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
            HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
            IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context);
            identityOptions = identityOptions ?? new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
            schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
            var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
            sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
            return sm;
        }

        // NECESITAMOS ESTAS 2 INSTANCIACIONES DE CLASES PARA PODER UTILIZARLAS, DEBIDO A QUE AMBAS EN
        // NUESTRO CÓDIGO SON CLASES CONCRETAS, Y NO INTERFACES DE LAS CUALES PODAMOS HACER UN MOCK



        // ÉSTE ES EL MÉTODO QUE NOS REALIZA UN MOCK DE IAUTHENTICATIONSERVICE
        private Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();     // aquí registramos el auth en el
                                                                                                                    // servicio de inyección de dependencias

            return auth;
        }
    }
}
