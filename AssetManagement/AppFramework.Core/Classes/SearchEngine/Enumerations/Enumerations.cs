namespace AppFramework.Core.Classes.SearchEngine.Enumerations
{
    public enum TimePeriodForSearch
    {
        History = 0,
        CurrentTime = 1
    }

    public enum ElementType
    {
        MultipleAsset,
        DynamicList
    }

    public enum ConcatenationOperation
    {
        And = 0,
        Or = 1
    }

    public enum SearchType
    {
        SearchByKeywords = 0,
        SearchByType = 1,
        SearchByContext,
        SearchByCategory,
        SearchByDocuments,
        SearchByBarcode
    }

    public enum SearchOperator
    {
        Equal,
        Less,
        LessEqual,
        Like,
        More,
        MoreEqual,
        StringLess,
        StringLessEqual,
        StringMore,
        StringMoreEqual
    }
}
