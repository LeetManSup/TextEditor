using System;

namespace TextEditorLib.Managers
{
    public class SearchReplaceManager
    {
        public static int FindText(string source, string searchText, int startIndex = 0, bool matchCase = false)
        {
            if (source == null || searchText == null)
                return -1;

            var comparsion = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            return source.IndexOf(searchText, startIndex, comparsion);
        }

        public string? ReplaceText(string source, string searchText, string replaceText, bool matchCase = false)
        {
            if (source == null || searchText == null || replaceText == null)
                return source;

            var comparsion = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            return ReplaceFirstOccurence(source, searchText, replaceText, comparsion);
        }

        private string ReplaceFirstOccurence(string source, string find, string replace, StringComparison comparsion)
        {
            int index = source.IndexOf(find, comparsion);
            if (index < 0)
                return source;

            return string.Concat(source.AsSpan(0, index), replace, source.AsSpan(index + find.Length));
        }
    }
}
