﻿using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using TranslatorLibrary.lang;
using TranslatorLibrary.LanguageCode;

namespace TranslatorLibrary.Translator
{
    public class GoogleCNTranslator : ITranslator
    {
        private string errorInfo = string.Empty;//错误信息

        public string TranslatorDisplayName { get { return Strings.GoogleCNTranslator; } }

        public string GetLastError()
        {
            return errorInfo;
        }

        public async Task<string?> TranslateAsync(string sourceText, string desLang, string srcLang)
        {
            srcLang = GetLanguageCode(new CultureInfo(srcLang));
            desLang = GetLanguageCode(new CultureInfo(desLang));

            string fun = string.Format(@"TL('{0}')", sourceText);

            string googleTransUrl = "https://translate.googleapis.com/translate_a/single?client=gtx&dt=t&sl=" + srcLang + "&tl=" + desLang + "&q=" + HttpUtility.UrlEncode(sourceText);

            var hc = TranslatorCommon.GetHttpClient();

            try
            {
                var ResultHtml = await hc.GetStringAsync(googleTransUrl);

                dynamic TempResult = System.Text.Json.JsonSerializer.Deserialize<dynamic>(ResultHtml, TranslatorCommon.JsonOP)!;

                string ResultText = "";

                for (int i = 0; i < TempResult[0].GetArrayLength(); i++)
                {
                    ResultText += TempResult[0][i][0];
                }

                return ResultText;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                errorInfo = ex.Message;
                return null;
            }
            catch (TaskCanceledException ex)
            {
                errorInfo = ex.Message;
                return null;
            }
        }
        public static string GetLanguageCode(CultureInfo cultureInfo)
        {
            return GoogleCNLanguageCodeConverter.GetLanguageCode(cultureInfo);
        }
        public void TranslatorInit(string param1 = "", string param2 = "") {; }
    }
}
