using System;
using Guineu.Expression;

namespace Guineu
{
	public enum ErrorAction
	{
		Resume,
		Ignore = Resume,
		Retry,
		Cancel
	}

	public enum ErrorCodes
	{
		FileNotFound = 1,
		FileIsInUse = 3,
		DataTypeMismatch = 9,
		Syntax = 10,
		InvalidArgument = 11,
		VariableNotFound = 12,
		AliasNotFound = 13,
		UnrecognizedCommand = 16,
		TableNumberInvalid = 17,
		AliasNameAlreadyInUse = 24,
		NoIndexOrderSet = 26,
		InvalidSubscript = 31,
		UnrecognizedKeyword = 36,
		ContinueWithoutLocate = 42,
		NoTableOpen = 52,
		MustSpecifyAdditionalParameters = 94,
		NestingError = 96,
		CannotUpdateCursor = 111,
		InvalidSetArgument = 231,
		NotAnArray = 232,
		InternalConsistency = 1000,
		FeatureNotAvailable = 1001,
		UserDefinedError = 1098,
		ErrorReadingFile = 1104,
		CommandIsMissingRequiredClause = 1221,
		TooFewArguments = 1229,
		TooManyArguments = 1230,
		ConnectionHandleInvalid = 1466,
		ConnectivityError = 1526,
		PropertyValueInvalid = 1560,
		FieldDoesNotAcceptNullValue = 1581,
		IndexTagIsNotFound = 1683,
		FileAccessDenied = 1705,
		DataTypeInvalid = 1732,
		ClassDefinitionNotFound = 1733,
		PropertyIsNotFound = 1734,
		IsMethodEventOrObject = 1737,
		PropertyIsReadOnly = 1743,
		ObjectClassInvalid = 1744,
		ObjectNameAlreadyExists = 1771,
		ObjectNotFound = 1923,
		NotAnObject = 1924,
		UnknownMember = 1925,
		CannotRedefine = 1930,
		ObjectNotContained = 1938,
		ErrorCodeInvalid = 1941,
		IllegalRedefinition = 1960
	}

	public partial class ErrorException : Exception
	{
		readonly ErrorCodes errorField;
    public ErrorCodes Error
		{
			get { return errorField; }
		}

		readonly string paramField;
		public string Param
		{
			get { return paramField; }
		}

		public ErrorException(ErrorCodes err)
		{
			errorField = err;
		}

		public ErrorException(ErrorCodes err, string param)
			: this(err)
		{
			paramField = param;
		}
		public ErrorException(ErrorCodes err, Nti name)
			: this(err)
		{
			paramField = name.ToString();
		}
	}
}