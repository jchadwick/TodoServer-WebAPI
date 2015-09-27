using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TodoApi.Models
{
    public class Todo
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public TodoState State { get; set; }
        public bool Completed
        {
            get
            {
                return State == TodoState.Completed;
            } set
            {
                State = value ? TodoState.Completed : TodoState.Active;
            }
        }
        public DateTime? CompletedDate { get; set; }

        [Required]
        public virtual string OwnerId { get; private set; }
        [Required]
        public virtual DateTime CreateDate { get; private set; }
        [Required]
        public virtual string IpAddress { get; private set; }

        public Todo()
        {
            State = TodoState.Active;
            CreateDate = DateTime.UtcNow;
            OwnerId = GetOwnerId();
            IpAddress = GetIpAddress();
        }


        protected internal static Func<string> GetOwnerId = GetCurrentIpAddress;
        protected internal static Func<string> GetIpAddress = GetCurrentIpAddress;

        private static string GetCurrentIpAddress()
        {
            var context = HttpContext.Current;
            if (context == null)
                return null;

            return context.Request.UserHostAddress;
        }
    }
}