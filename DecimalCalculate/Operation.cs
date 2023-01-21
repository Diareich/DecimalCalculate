using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecimalCalculate
{
    public enum Operation : int
    {
        /// <summary>
        /// Пустая
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Деление
        /// </summary>
        Devide = 1,

        /// <summary>
        /// Сложение
        /// </summary>
        Summa =2,

        /// <summary>
        /// Умножение
        /// </summary>
        Multiplication=3,

        /// <summary>
        /// Вычитание
        /// </summary>
        Substraction =4
    }
}
