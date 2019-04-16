using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Enums;

namespace BihuApiCore.Model.Response
{
    public class BaseResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public BaseResponse()
        {

        }

        public BaseResponse(int code) : this(code, "")
        {

        }

        public BaseResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public BaseResponse(BusinessStatusType code)
        {
            Code = (int) code;
            Message = EnumberHelper.GetEnumDescription(code);
        }

        public BaseResponse(BusinessStatusType code, string message)
        {
            Code = (int) code;
            Message =message;
        }

        public static BaseResponse GetBaseResponse(int code)
        {
            return new BaseResponse(code);
        }

        public static BaseResponse GetBaseResponse(int code, string message)
        {
            return new BaseResponse(code, message);
        }

        public static BaseResponse GetBaseResponse(BusinessStatusType code)
        {
            return new BaseResponse(code);
        }

        public static BaseResponse GetBaseResponse(BusinessStatusType code, string message)
        {
            return new BaseResponse(code, message);
        }

    }

    public class BaseResponse<T> : BaseResponse where T : class
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        public BaseResponse()
        {}

        public BaseResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public static BaseResponse<T> GetBaseResponse(int code, string message, T data)
        {
            return new BaseResponse<T>(code, message, data);
        }

        public static BaseResponse<T> GetBaseResponse(BusinessStatusType code, T data)
        {
            return new BaseResponse<T>((int)code, EnumberHelper.GetEnumDescription(code), data);
        }

        public static BaseResponse<T> GetBaseResponse(BusinessStatusType code, string message, T data)
        {
            return new BaseResponse<T>((int)code, message, data);
        }
    }

}
