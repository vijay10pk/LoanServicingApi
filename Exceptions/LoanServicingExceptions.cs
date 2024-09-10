using System;
namespace LoanServicingApi.Exceptions
{
	public class LoanServicingExceptions
	{
        public class NotFoundException : Exception
        {
            public NotFoundException()
            {
            }

            public NotFoundException(string message)
                : base(message)
            {
            }

            public NotFoundException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class ConflictException : Exception
        {
            public ConflictException()
            {
            }

            public ConflictException(string message)
                : base(message)
            {
            }

            public ConflictException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class CustomApplicationException : Exception
        {
            public CustomApplicationException()
            {
            }

            public CustomApplicationException(string message)
                : base(message)
            {
            }

            public CustomApplicationException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class UnauthorizedException : Exception
        {
            public UnauthorizedException()
            {
            }

            public UnauthorizedException(string message)
                : base(message)
            {
            }

            public UnauthorizedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}

