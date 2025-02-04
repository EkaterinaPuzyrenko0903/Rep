namespace Library
{
    public class Input
    {

        public string? Month { get; set; } //Месяц
        public int Year { get; set; }//Год
        public double Revenue { get; set; }//Выручка
        public string? PayCondition { get; set; }//Условие платежа
        public double Share { get; set; }//Доля,%
        public int NumberBranch { get; set; } //Код филиала(справочник)
        public string? NameBranch { get; set; } //Название филиала для ввода(справочник)
        public int NumberPayCond { get; set; } //Номер для условия платежа(справочник)
        public string? NamePayCond { get; set; } //Название для условия платежа(справочник)
        public string? Name { get; set; } //Название филиала (для таблиц)


    }
}