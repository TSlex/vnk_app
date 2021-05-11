using BLL.App.Services;
using BLL.Base;
using BLL.Contracts;
using BLL.Contracts.Services;
using DAL.App.Entities.Identity;
using DAL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.App
{
    public class AppBLL : BaseBLL<IAppUnitOfWork>, IAppBLL
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IAppBLL> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserProvider _userProvider;

        public AppBLL(IAppUnitOfWork unitOfWork, IConfiguration configuration, ILogger<IAppBLL> logger,
            UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager, IUserProvider userProvider) : base(unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userProvider = userProvider;
        }

        public IAttributeService Attributes => GetService<IAttributeService>(() => new AttributeService(UnitOfWork));

        public IAttributeTypeService AttributeTypes =>
            GetService<IAttributeTypeService>(() => new AttributeTypeService(UnitOfWork));

        public IOrderService Orders => GetService<IOrderService>(() => new OrderService(UnitOfWork));

        public ITemplateService Templates => GetService<ITemplateService>(() => new TemplateService(UnitOfWork));

        public IIdentityService Identity => GetService<IIdentityService>(() =>
            new IdentityService(_configuration, _logger, _userManager, _roleManager, _signInManager,
                _userProvider, UnitOfWork));
    }
}