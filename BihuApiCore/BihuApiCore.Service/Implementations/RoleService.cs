using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Models;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace BihuApiCore.Service.Implementations
{
    public class RoleService : IRoleService
    {

        private readonly ICompanyModuleRelationRepository _companyModuleRelationRepository;
        private readonly IRoleModuleRelationRepository _roleModuleRelationRepository;

        private readonly IMapper _mapper;

        public RoleService(ICompanyModuleRelationRepository companyModuleRelationRepository, IRoleModuleRelationRepository roleModuleRelationRepository, IMapper mapper)
        {
            _companyModuleRelationRepository = companyModuleRelationRepository;
            _roleModuleRelationRepository = roleModuleRelationRepository;
            _mapper = mapper;
        }

        #region RoleModuleByRoleIdAsync

        public async Task<List<ModuleTreeViewModel>> RoleModuleByRoleIdAsync(long roleId, long compId)
        {
            //get company module
            var allModule =await _companyModuleRelationRepository.CompanyModuleFullByIdAsync(compId);
            //get role module
            List<Modules> roleModule = new List<Modules>();
            if (roleId > 0)
            {
                roleModule = _roleModuleRelationRepository.RoleModuleFullById(roleId);
            }

            //get all level=1 module
            var parentModule = allModule.Where(t => t.ParentCode == "system_all" && t.ModuleLevel == 1).ToList();

            #region buid module tree (infinite recursion is better considering extend )

            #region recursion version

            List<ModuleTreeViewModel> moduleTree = parentModule.Select(c => new ModuleTreeViewModel()
            {
                ModuleCode = c.ModuleCode,
                ModuleName = c.ModuleName,
                ParentCode = c.ParentCode,
                ModuleType = (EnumModuleType)c.ModuleType,
                Plat = (EnumPlatformType)c.PlatformType,
                Status = roleId > 0 ? GetRoleModuleStatus(roleModule, c.ModuleCode) : "",
                Nodes = new List<ModuleTreeViewModel>()
            }).ToList();

            ModuleTreeRecursion(allModule, roleModule, moduleTree, roleId);

            #endregion

            #endregion

            return moduleTree;
        }


        #endregion



        #region get Role Module Tree By RoleId

        public List<ModuleTreeViewModel> RoleModuleByRoleId(long roleId, long compId)
        {
            //get company module
            var allModule = _companyModuleRelationRepository.CompanyModuleFullById(compId);
            //get role module
            List<Modules> roleModule = new List<Modules>();
            if (roleId > 0)
            {
                roleModule = _roleModuleRelationRepository.RoleModuleFullById(roleId);
            }

            //get all level=1 module
            var parentModule = allModule.Where(t => t.ParentCode == "system_all" && t.ModuleLevel == 1).ToList();

            #region buid module tree (infinite recursion is better considering extend )

            #region recursion version

            List<ModuleTreeViewModel> moduleTree = parentModule.Select(c => new ModuleTreeViewModel()
            {
                ModuleCode = c.ModuleCode,
                ModuleName = c.ModuleName,
                ParentCode = c.ParentCode,
                ModuleType = (EnumModuleType)c.ModuleType,
                Plat = (EnumPlatformType)c.PlatformType,
                Status = roleId > 0 ? GetRoleModuleStatus(roleModule, c.ModuleCode) : "",
                Nodes = new List<ModuleTreeViewModel>()
            }).ToList();

            ModuleTreeRecursion(allModule, roleModule, moduleTree, roleId);

            #endregion

            #region old code style

            //List<ModuleTreeViewModel> moduleTree = parentModule.Select(p => new ModuleTreeViewModel()
            //{
            //    ModuleCode = p.ModuleCode,
            //    ModuleName = p.ModuleName,
            //    ParentCode = p.ParentCode,
            //    ModuleType = p.ModuleType,
            //    Plat=p.PlatformType,
            //    Status = roleId > 0 ? GetRoleModuleStatus(roleModule, p.ModuleCode) : "",
            //    Nodes = allModule.Where(c => c.ParentCode == p.ModuleCode).Select(c => new ModuleTreeViewModel()
            //    {
            //        ModuleCode = c.ModuleCode,
            //        ModuleName = c.ModuleName,
            //        ParentCode = c.ParentCode,
            //        ModuleType = c.ModuleType,
            //        Plat=c.PlatformType,
            //        Status = roleId > 0 ? GetRoleModuleStatus(roleModule, c.ModuleCode) : "",
            //        Nodes = allModule.Where(b => b.ParentCode == c.ModuleCode).Select(b => new ModuleTreeViewModel()
            //        {
            //            ModuleCode = p.ModuleCode,
            //            ModuleName = p.ModuleName,
            //            ParentCode = p.ParentCode,
            //            ModuleType = p.ModuleType,
            //            Plat=p.PlatformType,
            //            Status = roleId > 0 ? GetRoleModuleStatus(roleModule, p.ModuleCode) : "",
            //            Nodes=new List<ModuleTreeViewModel>()
            //        }).ToList(),
            //    }).ToList()
            //}).ToList();

            #endregion

            #endregion

            return moduleTree;
        }

        #region assist method

        public void ModuleTreeRecursion(List<Modules> allModule, List<Modules> roleModule, List<ModuleTreeViewModel> parentModule, long roleId)
        {
            List<ModuleTreeViewModel> list = new List<ModuleTreeViewModel>();
            foreach (var item in parentModule)
            {
                item.Nodes = allModule.Where(c => c.ParentCode == item.ModuleCode).Select(c => new ModuleTreeViewModel()
                {
                    ModuleCode = c.ModuleCode,
                    ModuleName = c.ModuleName,
                    ParentCode = c.ParentCode,
                    ModuleType = (EnumModuleType)c.ModuleType,
                    Plat = (EnumPlatformType)c.PlatformType,
                    OrderBy = c.OrderBy,
                    Status = roleId > 0 ? GetRoleModuleStatus(roleModule, c.ModuleCode) : "",
                    Nodes = new List<ModuleTreeViewModel>()
                }).ToList();
                list.AddRange(item.Nodes);
            }
            if (list.Any())
            {
                ModuleTreeRecursion(allModule, roleModule, list, roleId);
            }
        }


        public string GetRoleModuleStatus(IList<Modules> list, string code)
        {
            return list.FirstOrDefault(t => t.ModuleCode == code) != null ? "modify" : "";
        }

        #endregion

        #endregion

        #region role module tree use old view model 

        public List<ManagerModuleViewModel> OldRoleModuleByRoleId(long roleId, long compId)
        {
            List<ModuleTreeViewModel> list = RoleModuleByRoleId(roleId, compId);

            var oldList = _mapper.Map<List<ManagerModuleViewModel>>(list);

            return oldList;

        }

        #endregion
    }
}
