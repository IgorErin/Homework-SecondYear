
open System

module Types =
    type PassedStatus =
        | Passed
        | PassedWithExpectedException of Exception
        | Ignored of string
    
    type FailedStatus =
        | ReceivedException
        | IgnoredWithMessage
        | BeforeFailed
        | AfterFailed
    
    [<Struct>]
    type CompatibleStatus =
        | Passed
        | ReceivedException
        | IgnoredWithMessage
        | BeforeFailed
        | AfterFailed
        
    [<Struct>]
    type IncompatibleStatus =
        | Constructor
        | Generic
        | SpecialName
        | Parameters
        | ReturnType
    
    type Method =
        | Compatible of CompatibleStatus
        | NotCompatible of IncompatibleStatus
        
    type Method