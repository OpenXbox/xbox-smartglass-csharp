using System;

namespace SmartGlass
{
    /// <summary>
    /// Text input scope.
    /// Used by TextChannel.
    /// </summary>
    public enum TextInputScope
    {
        Default,
        Url,
        FullFilePath,
        FileName,
        EmailUserName,
        EmailSmtpAddress,
        LogOnName,
        PersonalFullName,
        PersonalNamePrefix,
        PersonalGivenName,
        PersonalMiddleName,
        PersonalSurname,
        PersonalNameSuffix,
        PostalAddress,
        PostalCode,
        AddressStreet,
        AddressStateOrProvince,
        AddressCity,
        AddressCountryName,
        AddressCountryShortName,
        CurrencyAmountAndSymbol,
        CurrencyAmount,
        Date,
        DateMonth,
        DateDay,
        DateYear,
        DateMonthName,
        DateDayName,
        Digits,
        Number,
        OneChar,
        Password,
        TelephoneNumber,
        TelephoneCountryCode,
        TelephoneAreaCode,
        TelephoneLocalNumber,
        Time,
        TimeHour,
        TimeMinorSec,
        NumberFullWidth,
        AlphanumericHalfWidth,
        AlphanumericFullWidth,
        CurrencyChinese,
        Bopomofo,
        Hiragana,
        KatakanaHalfWidth,
        KatakanaFullWidth,
        Hanja,
        HangulHalfWidth,
        HangulFullWidth,
        Search,
        SearchTitleText,
        SearchIncremental,
        ChineseHalfWidth,
        ChineseFullWidth,
        NativeScript
    }
}
