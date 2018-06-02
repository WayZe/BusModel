﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel.Kernel;

namespace Model_Lab
{
    public partial class SmoModel : Model
    {
        // класс для события 1 - приход пассажира на остановку
        public class K1 : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события
            Pass ZP;
            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                var rec = new PassRec();                            //новая запись 
                rec.Z = ZP;                                         //передаём в созданный объект, объект заявки
                Model.VQ.Add(rec);                                  //добавляем в очередь

                var ev1 = new K1();                                 // создание объекта события
                ev1.ZP = ZP;                                        // передача библиотекаря в событие
                double dt1 = Model.GenPassAppear.GenerateValue();
                Model.PlanEvent(ev1, dt1);                          // планирование события 3
            }
        }

        // класс для события 2 - приход автобуса на остановку
        public class K2 : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события
            Pass ZB;
            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                Model.SA = 1;
                int KolPassOut = Model.GenKolPassOut.GenerateValue();

                var ev1 = new K1();                                 // создание объекта события
                ev1.ZP = ZP;                                        // передача библиотекаря в событие
                double dt1 = Model.GenPassAppear.GenerateValue();
                Model.PlanEvent(ev1, dt1);                          // планирование события 3
            }
        }

        /*
        // класс для события 2 - приход библиотекаря к столу заказов
        public class Event2_Librarian : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события

            public Reader R;
            public Librarian L;

            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                R = Rtemp;

                // занесение в файл трассировки записи об обработанном событии
                Model.Tracer.EventTrace(this, "K = 2" + "    NBL = " + (L.NBL + 1));

                // моделирование похода библиотекаря в хранилище
                double dt = Model.BL_Perem_Uniform_Generator.GenerateValue();

                // моделирование поиска библиотекарем книг
                if (R.NP == 0)
                {
                    for (int i = 0; i < Model.QVZP[this.L.NBL].Count.Value; i++)
                    {
                        dt += Model.BL_poisk_Normal_Generator.GenerateValue();
                    }
                }
                else
                {
                    for (int i = 0; i < Model.QVZS[this.L.NBL].Count.Value; i++)
                    {
                        dt += Model.BL_poisk_Normal_Generator.GenerateValue();
                    }
                }

                // моделирование возвращения библиотекаря к столу заказов
                dt += Model.BL_Vydacha_Uniform_Generator.GenerateValue();

                if (Model.QVZP[this.L.NBL].Count.Value > 0)
                {
                    var ready_Prep = this.Model.QVZP[this.L.NBL].Pop();

                    var ev3 = new Event3_Finish();                      // создание объекта события
                    ev3.L = this.L;                                     // передача библиотекаря в событие

                    Model.PlanEvent(ev3, dt);                           // планирование события 3

                    Model.Tracer.PlanEventTrace(ev3, "К = 3" + "    NBL = " + (L.NBL + 1) + "    tвх = " + ready_Prep.R.time_enter + "  NCH = " + ready_Prep.R.NCH + " NP = " + (ready_Prep.R.NP + 1));     //выводим в трассировку
                }

                if (Model.QVZS[this.L.NBL].Count.Value > 0)
                {
                    var ready_Stud = this.Model.QVZS[this.L.NBL].Pop();

                    var ev3 = new Event3_Finish();                      // создание объекта события
                    ev3.L = this.L;                                     // передача библиотекаря в событие

                    Model.PlanEvent(ev3, dt);                           // планирование события 3

                    Model.Tracer.PlanEventTrace(ev3, "К = 3" + "    NBL = " + (L.NBL + 1) + "    tвх = " + ready_Stud.R.time_enter + "  NCH = " + ready_Stud.R.NCH + " NP = " + (ready_Stud.R.NP + 1));     //выводим в трассировку
                }

                Model.TraceModel();                                 // вывод строки состояния
            }
        }

        // класс для события 3 - выдача заказа читателю
        public class Event3_Finish : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события

            public Reader R;
            public Librarian L;

            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                R = Rtemp;

                // есть читатели в очереди на выдачу?
                if (Model.QVZP[this.L.NBL].Count.Value > 0)
                {
                    var ready_Prep = this.Model.QVZP[this.L.NBL].Pop();

                    // сбор статистики по времени нахождения преподавателей в системе
                    Model.TNS[0].Value += Model.Time - ready_Prep.R.time_enter;

                    // занесение в файл трассировки записи об обработанном событии
                    Model.Tracer.EventTrace(this, "К = 3" + "    NBL = " + (L.NBL + 1) + "   tвх = " + string.Format("{0:0.000}", ready_Prep.R.time_enter) + "    NCH = " + ready_Prep.R.NCH + " NP = " + (ready_Prep.R.NP + 1));
                    ready_Prep.R.NCH++;
                    Model.KOP++;

                    var ev3 = new Event3_Finish();                  // создание объекта события
                    ev3.L = this.L;                                 // передача библиотекаря в событие

                    Model.PlanEvent(ev3, 0);                        // планирование события 3

                    Model.Tracer.PlanEventTrace(ev3, "К = 3" + "    NBL = " + (L.NBL + 1) + "   tвх = " + ready_Prep.R.time_enter + "   NCH = " + ready_Prep.R.NCH + "  NP = " + (ready_Prep.R.NP + 1));     //выводим в трассировку
                }

                if (Model.QVZS[this.L.NBL].Count.Value > 0)
                {
                    var ready_Stud = this.Model.QVZS[this.L.NBL].Pop();

                    // сбор статистики по времени нахождения студентов в системе
                    Model.TNS[1].Value += Model.Time - ready_Stud.R.time_enter;

                    // занесение в файл трассировки записи об обработанном событии
                    Model.Tracer.EventTrace(this, "К = 3" + "    NBL = " + (L.NBL + 1) + "   tвх = " + string.Format("{0:0.000}", ready_Stud.R.time_enter) + "    NCH = " + ready_Stud.R.NCH + " NP = " + (ready_Stud.R.NP + 1));
                    ready_Stud.R.NCH++;
                    Model.KOS++;

                    var ev3 = new Event3_Finish();                  // создание объекта события
                    ev3.L = this.L;                                 // передача библиотекаря в событие

                    Model.PlanEvent(ev3, 0);                        // планирование события 3

                    Model.Tracer.PlanEventTrace(ev3, "К = 3" + "    NBL = " + (L.NBL + 1) + "   tвх = " + ready_Stud.R.time_enter + "   NCH = " + ready_Stud.R.NCH + "  NP = " + (ready_Stud.R.NP + 1));     //выводим в трассировку
                }

                if ((Model.QVZP[this.L.NBL].Count.Value == 0) && (Model.QVZS[this.L.NBL].Count.Value == 0))
                {
                    Model.KK = 0;

                    while ((Model.VQ[0].Count.Value != 0) && (Model.KK < NMAX))
                    {
                        // выбрать читателя из входной очереди преподавателей
                        var next_Prep = Model.VQ[0].Pop();

                        Model.KK = Model.KK + 1;

                        // занести читателя в список QVZP
                        var QueueRecord = new ReaderRec();          // создание объекта элемента очереди
                        QueueRecord.R = next_Prep.R;                // передача в созданный объект объекта читателя
                        Model.QVZP[this.L.NBL].Add(QueueRecord);    // добавление элемента в очередь
                    }

                    while ((Model.VQ[1].Count.Value != 0) && (Model.KK < NMAX))
                    {
                        // выбрать читателя из входной очереди студентов
                        var next_Stud = Model.VQ[1].Pop();

                        Model.KK = Model.KK + 1;

                        // занести читателя в список QVZS
                        var QueueRecord = new ReaderRec();          // создание объекта элемента очереди
                        QueueRecord.R = next_Stud.R;                // передача в созданный объект объекта читателя
                        Model.QVZS[this.L.NBL].Add(QueueRecord);    // добавление элемента в очередь
                    }

                    Model.KZS[this.L.NBL].Value = Model.KK;

                    if ((Model.KK == 0) && (Model.VQ[0].Count.Value == 0) && (Model.VQ[1].Count.Value == 0))
                    {
                        this.L.time_osv = Model.Time;

                        if (Model.QBL.Count.Value < KBL)
                        {
                            // поместить библиотекаря в очередь свободных библиотекарей
                            var QueueRecord = new LibrarianRec();       // создание объекта элемента очереди
                            QueueRecord.L = this.L;                     // передача в созданный объект объекта читателя
                            Model.QBL.Add(QueueRecord);                 // добавление элемента в очередь

                            Model.LQBL = Model.QBL.Count.Value;
                        }
                    }

                    if (Model.KK != 0)
                    {
                        var ev2 = new Event2_Librarian();           // создание объекта события
                        ev2.L = this.L;

                        Model.PlanEvent(ev2, 0);                    // планирование события 2
                        Model.Tracer.PlanEventTrace(ev2, "K = 2" + "    NBL = " + (L.NBL + 1));
                    }
                }

                Model.TraceModel();                                 // вывод строки состояния
            }
            
        }*/
    }
}