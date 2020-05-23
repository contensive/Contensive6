
namespace Contensive.Processor.Addons.AdminSite {
    //
    //====================================================================================================
    /// <summary>
    /// admin filter methods (includes, equals, greaterthan, etc)
    /// </summary>
    public enum FindWordMatchEnum {
        MatchIgnore = 0,
        MatchEmpty = 1,
        MatchNotEmpty = 2,
        MatchGreaterThan = 3,
        MatchLessThan = 4,
        matchincludes = 5,
        MatchEquals = 6,
        MatchTrue = 7,
        MatchFalse = 8
    }
}
