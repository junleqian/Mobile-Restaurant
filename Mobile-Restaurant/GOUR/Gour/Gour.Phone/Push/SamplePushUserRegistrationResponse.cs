namespace Gour.Phone.Push
{
    using System;

    public class SamplePushUserRegistrationResponse<T> : Exception
    {
        private T response;
        private Exception exception;

        public SamplePushUserRegistrationResponse()
            : base()
        {
        }

        public SamplePushUserRegistrationResponse(T response, Exception exception)
        {
            this.response = response;
            this.exception = exception;
        }

        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        public T Response
        {
            get
            {
                return this.response;
            }
        }
    }
}
