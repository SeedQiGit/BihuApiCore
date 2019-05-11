using System.ComponentModel.DataAnnotations;

namespace BihuApiCore.Model.Request
{
    public class UpdateUserRequest:BaseRequest
    {
        /// <summary>
        /// 父级id
        /// </summary>
       [Required,Range(1,long.MaxValue)] 
        public long ParentId { get; set; }
    }
}
