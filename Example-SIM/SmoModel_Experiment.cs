using System;
using CommonModel.Kernel;
using CommonModel.RandomStreamProducing;
using System.Collections.Generic;

namespace Model_Lab
{

    public partial class SmoModel : Model
    {
        //Условие завершения прогона модели True - завершить прогон. По умолчанию false. </summary>
        public override bool MustStopRun(int variantCount, int runCount)
        {
            return (Time >= TP);
        }

        //установка метода перебора вариантов модели
        public override bool MustPerformNextVariant(int variantCount)
        {
            //используем один вариант модели
            return variantCount < 1;
        }

        //true - продолжить выполнение ПРОГОНОВ модели;
        //false - прекратить выполнение ПРОГОНОВ модели. по умолчению false.
        public override bool MustPerformNextRun(int variantCount, int runCount)
        {
            return runCount < 1; //выполняем 1 прогон модели
        }

        //Задание начального состояния модели для нового варианта модели
        public override void SetNextVariant(int variantCount)
        {
            #region Параметры модели
            LAMBD = 1.0 / 2;
            SA = 0;
            #endregion

            #region Установка параметров законов распределения

            (GenPassAppear.BPN as GeneratedBaseRandomStream).Seed = 1;
            (GenKolPassOut.BPN as GeneratedBaseRandomStream).Seed = 2;

            GenPassAppear.Lyambda = LAMBD;
            GenKolPassOut.Table = new Dictionary<int, double>();
            for (int i = VL; i <= VP; i++)
                GenKolPassOut.Table.Add(i, 1.0/(VP-VL+1));

            /*
         
            BL_Perem_Uniform_Generator.A = AP;
            BL_Perem_Uniform_Generator.B = BP;

            BL_Vydacha_Uniform_Generator.A = AZ;
            BL_Vydacha_Uniform_Generator.B = BZ;

            BL_poisk_Normal_Generator.Mx = MO;
            BL_poisk_Normal_Generator.Sigma = SKO;
            */
            #endregion
        }

        public override void StartModelling(int variantCount, int runCount)
        {
            #region Задание начальных значений модельных переменных и объектов

            KVP = 0;                        // количество вошедших преподавателей
            KVS = 0;                        // количество вошедших студентов
            KOP = 0;                        // количество обслуженных преподавателей
            KOS = 0;                        // количество обслуженных студентов

            LVQP = VQ[0].Count;             // длина входной очереди преподавателей
            LVQS = VQ[1].Count;             // длина входной очереди студентов
            LQBL = KBL;                     // длина очереди свободных библиотекарей

            TNS[0].Value = 0.0;             // время нахождения преподавателей в системе
            TNS[1].Value = 0.0;             // время нахождения студентов в системе

            TSPBL = 0.0;                    // суммарное время простоя библиотекарей

            for (int i = 0; i < QUEUE; i++)
            {
                VQ[i].Clear();
            }

            for (int i = 0; i < KBL; i++)
            {
                Librarian libr = new Librarian();
                libr.NBL = i;
                libr.time_osv = 0.0;

                var QueueRecord = new LibrarianRec();           // создание объекта элемента очереди
                QueueRecord.L = libr;                           // передача в созданный объект объекта читателя
                QBL.Add(QueueRecord);                           // добавление элемента в очередь

                QVZP[i].Clear();
                QVZS[i].Clear();

                KZS[i].Value = 0;
            }

            #endregion

            #region Cброс сборщиков статистики

            for (int i = 0; i < QUEUE; i++)
            {
                Variance_TNS[i].ResetCollector();
            }

            for (int i = 0; i < KBL; i++)
            {
                Variance_KZS[i].ResetCollector();
            }

            #endregion

            //Печать заголовка строки состояния модели
            TraceModelHeader();

            #region Планирование начальных событий

            Reader Prep = new Reader();
            Prep.NCH = 1;
            Prep.NP = 0;
            var start_event_Prep = new Event1_Enter();
            start_event_Prep.R = Prep;
            PlanEvent(start_event_Prep, 0.0);

            Reader Stud = new Reader();
            Stud.NCH = 1;
            Stud.NP = 1;
            var start_event_Stud = new Event1_Enter();
            start_event_Stud.R = Stud;
            PlanEvent(start_event_Stud, 0.0);

            #endregion
        }

        //Действия по окончанию прогона
        public override void FinishModelling(int variantCount, int runCount)
        {
            Tracer.AnyTrace("");
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("============Статистические результаты моделирования===========");
            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");
            Tracer.TraceOut("Время моделирования: " + string.Format("{0:0.00}", Time));
        }

        //Печать заголовка
        void TraceModelHeader()
        {
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("======================= Запущена модель ======================");
            Tracer.TraceOut("==============================================================");
            //вывод заголовка трассировки
            Tracer.AnyTrace("");
            Tracer.AnyTrace("Параметры модели:");

            Tracer.AnyTrace("Начальное состояние модели:");

            // для 1 библиотекаря
            //Tracer.AnyTrace("KVP = " + KVP, "KVS = " + KVS, "KOP = " + KOP, "KOS = " + KOS,
            //    "LVQP = " + VQ[0].Count.Value, "LVQS = " + VQ[1].Count.Value, "LQBL = " + QBL.Count.Value, "TSPBL = " + string.Format("{0:0.00}", TSPBL),
            //    "QVZP = [" + QVZP[0].Count.Value + "]", "QVZS = [" + QVZS[0].Count.Value + "]"); Tracer.AnyTrace("");

            // для 2 библиотекарей
            Tracer.AnyTrace("KVP = " + KVP, "KVS = " + KVS, "KOP = " + KOP, "KOS = " + KOS,
                "LVQP = " + VQ[0].Count.Value, "LVQS = " + VQ[1].Count.Value, "LQBL = " + QBL.Count.Value, "TSPBL = " + string.Format("{0:0.00}", TSPBL),
                "QVZP = [" + QVZP[0].Count.Value + "; " + QVZP[1].Count.Value + "]", "QVZS = [" + QVZS[0].Count.Value + "; " + QVZS[1].Count.Value + "]"); Tracer.AnyTrace("");

            // для 5 библиотекарей
            //Tracer.AnyTrace("KVP = " + KVP, "KVS = " + KVS, "KOP = " + KOP, "KOS = " + KOS,
            //    "LVQP = " + VQ[0].Count.Value, "LVQS = " + VQ[1].Count.Value, "LQBL = " + QBL.Count.Value, "TSPBL = " + string.Format("{0:0.00}", TSPBL),
            //    "QVZP = [" + QVZP[0].Count.Value + "; " + QVZP[1].Count.Value + "; " + QVZP[2].Count.Value + "; " + QVZP[3].Count.Value + "; " + QVZP[4].Count.Value + "]",
            //    "QVZS = [" + QVZS[0].Count.Value + "; " + QVZS[1].Count.Value + "; " + QVZS[2].Count.Value + "; " + QVZS[3].Count.Value + "; " + QVZS[4].Count.Value + "]"); 

            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");
        }

        //Печать строки состояния
        void TraceModel()
        {
            // для 1 библиотекаря
            //Tracer.AnyTrace("KVP = " + KVP, "KVS = " + KVS, "KOP = " + KOP, "KOS = " + KOS,
            //    "LVQP = " + VQ[0].Count.Value, "LVQS = " + VQ[1].Count.Value, "LQBL = " + QBL.Count.Value, "TSPBL = " + string.Format("{0:0.00}", TSPBL),
            //    "QVZP = [" + QVZP[0].Count.Value + "]", "QVZS = [" + QVZS[0].Count.Value + "]"); Tracer.AnyTrace("");

            // для 2 библиотекарей
            Tracer.AnyTrace("KVP = " + KVP, "KVS = " + KVS, "KOP = " + KOP, "KOS = " + KOS,
                "LVQP = " + VQ[0].Count.Value, "LVQS = " + VQ[1].Count.Value, "LQBL = " + QBL.Count.Value, "TSPBL = " + string.Format("{0:0.00}", TSPBL),
                "QVZP = [" + QVZP[0].Count.Value + "; " + QVZP[1].Count.Value + "]", "QVZS = [" + QVZS[0].Count.Value + "; " + QVZS[1].Count.Value + "]");

            // для 5 библиотекарей
            //Tracer.AnyTrace("KVP = " + KVP, "KVS = " + KVS, "KOP = " + KOP, "KOS = " + KOS,
            //    "LVQP = " + VQ[0].Count.Value, "LVQS = " + VQ[1].Count.Value, "LQBL = " + QBL.Count.Value, "TSPBL = " + string.Format("{0:0.00}", TSPBL),
            //    "QVZP = [" + QVZP[0].Count.Value + "; " + QVZP[1].Count.Value + "; " + QVZP[2].Count.Value + "; " + QVZP[3].Count.Value + "; " + QVZP[4].Count.Value + "]",
            //    "QVZS = [" + QVZS[0].Count.Value + "; " + QVZS[1].Count.Value + "; " + QVZS[2].Count.Value + "; " + QVZS[3].Count.Value + "; " + QVZS[4].Count.Value + "]"); 
        }

    }
}

