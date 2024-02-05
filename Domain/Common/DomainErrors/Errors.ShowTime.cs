using ErrorOr;

namespace ApiApplication.Domain.Common.DomainErrors
{

    public static partial class Errors
    {
        public static class ShowTimes
        {
            public static Error InvalidMenuId => Error.Validation(
                code: "Menu.InvalidId",
                description: "Menu ID is invalid");

            public static Error NotFound => Error.NotFound(
                code: "Menu.NotFound",
                description: "Menu with given ID does not exist");
        }

        public static class Auditoriums
        {
            public static Error InvalidMenuId => Error.Validation(
                code: "Menu.InvalidId",
                description: "Menu ID is invalid");

            public static Error NotFound => Error.NotFound(
                code: "Menu.NotFound",
                description: "Menu with given ID does not exist");
        }
    }

}