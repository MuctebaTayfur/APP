using System;

using System.Runtime.Serialization;

namespace APP.Base.Model.Enum
{
    public enum Status : short
    {
        [EnumMember(Value = "PASSIVE")]
        Passive,
        [EnumMember(Value = "ACTIVE")]
        Active,
        [EnumMember(Value = "MODIFIED")]
        Modified,
        [EnumMember(Value = "DELETED")]
        Deleted,
        [EnumMember(Value = "DELETED_FROM_TRASH")]
        DeletedFromTrash

    }

    public enum Role : short
    {
        Owner,
        Admin,
        Member,
        Driver,
        Expert,
        TeamLeader,
        Supervisor

    }

    public enum RequestType
    {
        POST,
        GET,
        PUT,
        DELETE
    }

    public enum ItemFieldType 
    {
        MinLevel,
        Quantity,
        CustomField
    }

    public enum AlarmType 
    {
        Up,
        Down
    }

    public enum OrderType 
    {
        Asc,
        Desc
    }

    public enum CustomFieldType 
    {
        Inputable,
        Selectable,
        Checkable,
        Dateble,
        Uploadable
    }

    public enum CustomFieldSelectableType 
    {
        Single,
        Multiple
    }

    public enum CustomFieldInputableType 
    {
        Numeric,
        Text
    }

    public enum MinLevelType 
    {
        AtOrBelowMinLevel,
        AboveMinLevel,
        WithMinLevelSet

    }

    public enum Authority : short
    {
        CanEdit,
        CanReview
    }
}
