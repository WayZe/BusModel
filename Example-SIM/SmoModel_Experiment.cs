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
            LAMBD = 1.0/2;
            SA = 0;
			B = 25;
			T = 10.0;
			A = 1.0;
            TP = 200;
            NVAR = 1;
            ml = 20;
            mp = 25;
            bcl = 0.0;
            bcp = 1.0;
            pcl = 0.0;
            pcp = 1.0;
            VL = 1;
            VP = 7;
            #endregion

            #region Установка параметров законов распределения

            (GenPassAppear.BPN as GeneratedBaseRandomStream).Seed = 1;
            (GenPassOut.BPN as GeneratedBaseRandomStream).Seed = 2;
            (GenBusAppear.BPN as GeneratedBaseRandomStream).Seed = 3;
            (GenPassIn.BPN as GeneratedBaseRandomStream).Seed = 4;

            GenPassAppear.Lyambda = LAMBD;

            GenPassOut.A = bcl;
            GenPassOut.B = bcp;

            GenPassIn.A = pcl;
            GenPassIn.B = pcp;

            GenBusAppear.A = -A;
			GenBusAppear.B = A;

            #endregion
        }

        public override void StartModelling(int variantCount, int runCount)
        {
            #region Задание начальных значений модельных переменных и объектов
            #endregion

            #region Cброс сборщиков статистики

            #endregion

            //Печать заголовка строки состояния модели
            TraceModelHeader();

            #region Планирование начальных событий
            
            var ev1 = new K1();                                 // создание объекта события
            Pass Z1 = new Pass();
            Z1.NZ = 1;
            ev1.ZP = Z1;                                        // передача библиотекаря в событие
            PlanEvent(ev1, 0.0);                          // планирование события 3
			Tracer.PlanEventTrace(ev1);

            Random rnd = new Random();
            var ev2 = new K2();                                 // создание объекта события
            Bus Z2 = new Bus();
            Z2.NB = 1;
			KPA = 0;
            Z2.KPA = KPA;
            ev2.ZB = Z2;                                      // передача библиотекаря в событие
            double dt2 = T + GenBusAppear.GenerateValue();
            PlanEvent(ev2, dt2);                          // планирование события 3
			Tracer.PlanEventTrace(ev2);

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

			Tracer.TraceOut("Статистические характеристики длины очереди: ");
			Tracer.TraceOut("МО = " + Variance_LQ.Mx.ToString("#.###"));
			Tracer.TraceOut("Дисперсия = " + Variance_LQ.Stat.ToString("#.###"));

			Tracer.TraceOut("");
			Tracer.TraceOut("KNP = " + KNP);

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
            Tracer.AnyTrace("Интенсивность потока пассажиров:");
            Tracer.AnyTrace("LAMBD = " + LAMBD );
            Tracer.AnyTrace("Состояние автобуса:");
            Tracer.AnyTrace("SA = " + SA    );
            Tracer.AnyTrace("Размер автобуса:");
            Tracer.AnyTrace("B = " + B     );
            Tracer.AnyTrace("Интервал времени прибытия автобуса:");
            Tracer.AnyTrace("T = " + T     );
            Tracer.AnyTrace("Погрешность прибытия автобуса:");
            Tracer.AnyTrace("A = " + A     );
            Tracer.AnyTrace("Время прогона:");
            Tracer.AnyTrace("TP = " + TP    );
            Tracer.AnyTrace("Вариант модели:");
            Tracer.AnyTrace("NVAR = " + NVAR  );
            Tracer.AnyTrace("Левая и правая границы числа пассажиров в автобусе:");
            Tracer.AnyTrace("ml = " + ml    );
            Tracer.AnyTrace("mp = " + mp    );
            Tracer.AnyTrace("Левая и правая границы времени высадки пассажира:");
            Tracer.AnyTrace("bcl = " + bcl   );
            Tracer.AnyTrace("bcp = " + bcp   );
            Tracer.AnyTrace("Левая и правая границы времени посадки пассажира:");
            Tracer.AnyTrace("pcl = " + pcl   );
            Tracer.AnyTrace("pcp = " + pcp);
            Tracer.AnyTrace("Левая и правая границы количества выходящих пассажиров из автобуса:");
            Tracer.AnyTrace("VL = " + VL);
            Tracer.AnyTrace("VP = " + VP);
            Tracer.AnyTrace("");

            Tracer.AnyTrace("Начальное состояние модели:");
            TraceModel();
            Tracer.AnyTrace("");

            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");
        }

        //Печать строки состояния
        void TraceModel()
        {
			Tracer.AnyTrace("KVZ = " + KVZ + " SA = " + SA + " KPA = " + KPA + " KA = " + KA + " LQ = " + LQ.Value);
        }      
    }
}

