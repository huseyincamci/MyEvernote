using MyEvernote.Entities.Messages;
using System.Collections.Generic;

namespace MyEvernote.BusinessLayer.Results
{
    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMessage> Errors { get; set; }
        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessage>();
        }

        public void AddError(ErrorMessagesCode code, string message)
        {
            Errors.Add(new ErrorMessage() { Code = code, Message = message});
        }
    }
}
