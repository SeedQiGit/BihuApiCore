using System;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Enums;

namespace BihuApiCore.Model.Response
{
    public class BaseResponse
    {
        #region 属性

        public int Code { get; set; }
        public string Message { get; set; }

        #endregion

        #region 构造函数

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
            Code = (int)code;
            Message = EnumberHelper.GetEnumDescription(code);
        }

        public BaseResponse(BusinessStatusType code, string message)
        {
            Code = (int)code;
            Message = message;
        }

        #endregion

        #region 扩展方法

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

        #endregion

        #region 快捷方法

        /// <summary>
        ///     返回成功结果
        /// </summary>
        /// <param name="message">结果信息</param>
        /// <returns></returns>
        public static BaseResponse Failed(string message = null)
        {
            var code = BusinessStatusType.Failed;
            return GetBaseResponse(code, message ?? EnumberHelper.GetEnumDescription(code));
        }

        /// <summary>
        ///     返回成功结果
        /// </summary>
        /// <param name="message">结果信息</param>
        /// <returns></returns>
        public static BaseResponse Ok(string message = null)
        {
            var code = BusinessStatusType.OK;
            return GetBaseResponse(code, message ?? EnumberHelper.GetEnumDescription(code));
        }

        /// <summary>
        ///     返回错误结果
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static BaseResponse Error(Exception ex)
        {
            return Error(ex.Message);
        }

        /// <summary>
        ///     返回错误结果
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static BaseResponse Error(string message = null)
        {
            var code = BusinessStatusType.Error;
            return GetBaseResponse(code, message ?? EnumberHelper.GetEnumDescription(code));
        }

        #endregion      

    }

    public class BaseResponse<T> : BaseResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        #region 构造函数
        public BaseResponse()
        { }

        public BaseResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        #endregion

        #region 扩展方法

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

        #endregion

        #region 快捷方法

        /// <summary>
        ///     返回成功结果
        /// </summary>
        /// <param name="data">结果数据集</param>
        /// <returns></returns>
        public static BaseResponse<T> Ok(T data)
        {
            var code = BusinessStatusType.OK;
            return GetBaseResponse(code, data);
        }

        #endregion

    }

}
