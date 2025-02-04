using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class OutPut
    {

        public string? Month { get; set; } //Месяц
        public int Year { get; set; }//Год
        public double Revenue { get; set; }//Выручка
        public string? PayCondition { get; set; }//Условие платежа
        public double Share { get; set; }//Доля,%
        public int NumberBranch { get; set; } //Код филиала (справочник)
        public string? NameBranch { get; set; }//Название филиала для ввода(справочник)
        public int NumberPayCond { get; set; } //Номер для условия платежа(справочник)
        public string? NamePayCond { get; set; } //Название для условия платежа(справочник)
        public string? Name { get; set; } //Название филиала (для таблиц)
        public OutPut(Input input)
        {
        
            Month = input.Month;
            Year = input.Year;
            Revenue = input.Revenue;
            PayCondition = input.PayCondition;
            Share = input.Share;
            NumberBranch = input.NumberBranch;
            NameBranch = input.NameBranch;
            NumberPayCond = input.NumberPayCond;
            NamePayCond = input.NamePayCond;
            Name = input.Name;
        }
    }
}
