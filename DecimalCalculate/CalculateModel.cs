using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DecimalCalculate
{
    /// <summary>
    /// Класс реализации калькулятора
    /// </summary>
    public class CalculateModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> action)
        {
            if (object.Equals((object)storage, (object)value))
                return false;
            storage = value;
            this.RaisePropertyChanged<T>(action);
            return true;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            this.RaisePropertyChanged(CalculateModel.GetPropertyName<T>(action));
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            return ((MemberExpression)action.Body).Member.Name;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public CalculateModel()
        {
            Numerator = 3;
            IsNumerator = false;
            Operations = Operation.Empty;
        }

        private Operation operations;

        public Operation Operations
        {
            get
            {
                return operations;
            }
                set
            {
                this.SetProperty<Operation>(ref this.operations, value, (Expression<Func<Operation>>)(() => this.Operations));
                EqualyCommand.RaiseCanExecuteChanged();
                string op = string.Empty;
                switch (value)
                {
                    case Operation.Empty:
                        op = "Пустой";
                        break;
                    case Operation.Devide:
                        op = "Деление";
                        break;
                    case Operation.Multiplication:
                        op = "Умножение";
                        break;
                    case Operation.Substraction:
                        op = "Вычитание";
                        break;
                    case Operation.Summa:
                        op = "Сложение";
                        break;
                }
                WriteDiagnostic(string.Format("Ввод Операции {0}", op));
            }
        }

        string patch = @"Diagnostic.txt";

        private int? numerator;

        /// <summary>
        /// числитель
        /// </summary>
        public int? Numerator
        {
            get
            {
                return numerator;
            }
            set
            {
                this.SetProperty<int?>(ref this.numerator, value, (Expression<Func<int?>>)(() => this.Numerator));
                if (value.HasValue)
                    WriteDiagnostic(string.Format("Ввод числителя {0}", value));
                else
                    WriteDiagnostic("Сброс числителя ");
            }
        }

        private bool isNumerator;

        /// <summary>
        /// Определяет ввод числитель или знаменатель
        /// </summary>
        public bool IsNumerator
        {
            get
            {
                return isNumerator;
            }
            set
            {
                this.SetProperty<bool>(ref this.isNumerator, value, (Expression<Func<bool>>)(() => this.IsNumerator));
                isNumerator = value;
                TextInput = value ? "числитель" : "знаменатель";
            }
        }

        private string textInput;

        public string TextInput
        {
            get { return textInput; }
            set
            {
                this.SetProperty<string>(ref this.textInput, value, (Expression<Func<string>>)(() => this.TextInput));
            }
        }
        
        private int? denominator;

        /// <summary>
        /// знаменатель
        /// </summary>
        public int? Denominator
        {
            get
            {
                return denominator;
            }
            set
            {
                this.SetProperty<int?>(ref this.denominator, value, (Expression<Func<int?>>)(() => this.Denominator));
                if( value.HasValue)
                WriteDiagnostic(string.Format("Ввод знаменателя {0}", value));
                else
                    WriteDiagnostic("Сброс знаменателя ");
            }
        }


        private string viewText;

        public string ViewText
        {
            get { return viewText; }
            set { viewText = value; }
        }


        /// <summary>
        /// Запись данных в протокол
        /// </summary>
        private void WriteDiagnostic(string data)
        {            
            File.AppendAllText(patch, data);
            File.AppendAllText(patch, Environment.NewLine);
        }

        private DelegateCommand equalyCommand;

        /// <summary>
        /// Нажатие =
        /// </summary>
        public DelegateCommand EqualyCommand
        {
            get
            {
                Func < bool > b = ()=> Operations != Operation.Empty;
                return this.equalyCommand ?? (this.equalyCommand = new DelegateCommand(new Action<object>(this.EqualyExecute), (b)  ));
            }
        }

        private void EqualyExecute(object p)
        {
            switch (Operations)
            {
                case Operation.Empty:
                    return;
                case Operation.Devide:
                    DevideOperation();
                    break;
                case Operation.Multiplication:
                    MultiOperation();
                    break;
                case Operation.Substraction:
                    SubstrictionOperation();
                    break;
                case Operation.Summa:
                    SummaOperation();
                    break;
            }
            Operations = Operation.Empty;
        }

        private void SummaOperation()
        {
            NumeratorFirst = NumeratorFirst * Denominator.Value;
            DenumeratorFirst = DenumeratorFirst * Denominator.Value;
            Numerator = Numerator.Value * DenumeratorFirst + NumeratorFirst * Denominator;
            Denominator = Denominator.Value * DenumeratorFirst;
        }

        private void SubstrictionOperation()
        {
            NumeratorFirst = NumeratorFirst * Denominator.Value;
            DenumeratorFirst = DenumeratorFirst * Denominator.Value;
            Numerator = Numerator.Value * DenumeratorFirst - NumeratorFirst *Denominator;
            Denominator = Denominator.Value * DenumeratorFirst;
        }

        private void MultiOperation()
        {   
            Numerator = Numerator.Value * DenumeratorFirst;
            Denominator = Denominator.Value * DenumeratorFirst;
        }

        private void DevideOperation()
        {
            Numerator = Denominator.Value * DenumeratorFirst;
            Denominator = Numerator.Value * DenumeratorFirst;
        }


        private DelegateCommand numberCommand;

        /// <summary>
        /// Нажатие Один
        /// </summary>
        public DelegateCommand NumberCommand
        {
            get
            {
                return this.numberCommand ?? (this.numberCommand = new DelegateCommand(new Action<object>(this.OneExecute ), (Func<bool>)null));
            }
        }

        private void OneExecute(object param)
        {
            int ad = int.Parse(param.ToString());
            AddNumber(ad);
        }

        private DelegateCommand operationCommand;

        /// <summary>
        /// Нажатие Один
        /// </summary>
        public DelegateCommand OperationCommand
        {
            get
            {
                return this.operationCommand ?? (this.operationCommand = new DelegateCommand(new Action<object>(this.DivideExecute), (Func<bool>)null));
            }
        }

        private void DivideExecute(object p)
        {
            int op = int.Parse(p.ToString());
            Operations = (Operation)op;
            NumeratorFirst = Numerator.Value;
            DenumeratorFirst = Denominator.Value;
            Numerator = null;
            Denominator = null;
        }

        public int NumeratorFirst { get; set; }

        public int DenumeratorFirst { get; set; }

        private DelegateCommand viewCommand;

        /// <summary>
        /// Нажатие Один
        /// </summary>
        public DelegateCommand ViewCommand
        {
            get
            {
                return this.viewCommand ?? (this.viewCommand = new DelegateCommand(new Action<object>(this.ViewExecute), (Func<bool>)null));
            }
        }

        private void ViewExecute(object p)
        {
            var win = new ViewWindow(patch);
            win.ShowDialog();
        }

        private DelegateCommand changeCommand;

        /// <summary>
        /// Нажатие Один
        /// </summary>
        public DelegateCommand ChangeCommand
        {
            get
            {
                return this.changeCommand ?? (this.changeCommand = new DelegateCommand(new Action<object>(this.ChangeExecute), (Func<bool>)null));
            }
        }

        private void ChangeExecute(object p)
        {
            int? d = Denominator;
            if (Numerator.HasValue && Numerator > 0)
            {
                Denominator = Numerator;
                Numerator = d;
            }
            else
            {
                Denominator = Numerator *-1;
                Numerator = d * (-1);
            }
        }

        private DelegateCommand clearCommand;

        /// <summary>
        /// Нажатие Один
        /// </summary>
        public DelegateCommand ClearCommand
        {
            get
            {
                return this.clearCommand ?? (this.clearCommand = new DelegateCommand(new Action<object>(this.ClearExecute), (Func<bool>)null));
            }
        }

        private void ClearExecute(object p)
        {
            Denominator= null;
            Numerator = null;
        }
        
        

        private void AddNumber(int ad)
        {
            if (this.IsNumerator)
            {
                CreateNumerator();
                Numerator = Numerator * 10 + ad;
            }
            else
            {
                CreateDenominator();
                Denominator = Denominator * 10 + ad;
            }
        }

        private void CreateNumerator()
        {
            if (!Numerator.HasValue)
                Numerator = 0;
        }

        private void CreateDenominator()
        {
            if (!Denominator.HasValue)
                Denominator = 0;
        }



    }
}
