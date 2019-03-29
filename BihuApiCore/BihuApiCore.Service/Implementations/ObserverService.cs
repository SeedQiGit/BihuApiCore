using AutoMapper;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class ObserverService:IObserverService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;
 
        private UrlModel _urlModel;

        public ObserverService(IUserRepository userRepository, IMapper mapper , IOptions<UrlModel> option )
        {
            _userRepository = userRepository;
            _mapper = mapper;

            _urlModel = option.Value;
        }

        public async Task<BaseResponse> AddUserAllSheet(BaseRequest request)
        {
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
        }
    }
}
