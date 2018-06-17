using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonModel.StatisticsCollecting;
using CommonModel.RandomStreamProducing;
using CommonModel.Collections;
using CommonModel.Kernel;

namespace Model_Lab
{

    public partial class SmoModel : Model
    {

        #region Параметры модели
        // Интенсивность входного потока пассажиров
        double LAMBD;
        // Левая и правая границы числа пассажиров в автобусе в момент прихода
        int ml, mp;
        // Левая и правая границы высадки пассажира
        double bcl, bcp;
        // Левая и правая границы посадки пассажира
        double pcl, pcp;
        // Интервал прихода автобуса
        double T;
        // Отклонение прихода автобуса
        double A;
        // Вместительность автобуса
        int B;
        // Время прогона
        double TP;
        // Левая и правая границы количества выходящих пассажиров из автобуса
        int VL, VP;
        // Номер варианта
        int NVAR;


        // Количество оставшихся пассажиров в автобусе после высадки
        int KOP;
        // Список времени нахождения в очереди каждого пассажира
        List<double> times = new List<double>();
        // Счетчик вошедших пассажиров
        int iPassIn = -1;
        // Счетчик вышедших пассажиров
        int iPassOut = -1;
        #endregion

        #region Переменные состояния модели

        // Количество пришедших на остановку пассажиров
        int KVZ;
        // Состояние автобуса
        int SA;
        // Количество пассажиров в автобусе
        int KPA;
        // Количество автобусов, пришедших на остановку
        int KA;

		TIntVar LQ;
        TRealVar TQ;
		int KNP = 0;
        #endregion

        #region Дополнительные структуры



        // Заявки в узлах ВС
        public class Pass
        {
            // Сквозной номер пассажира
            public int NZ;
        }

        // Созданные ПП в узлах ВС
        public class Bus
        {
            // Номер автобуса
            public int NB;
            // Количество пассажиров в автобусе
            public int KPA;
        }

        // Элемент очереди заявки в узлах ВС 
        class PassRec : QueueRecord
        {
            public Pass Z;
        }

        // группа списков для определения состояния входных очередей заявок в узлах ВС
        SimpleModelList<PassRec> VQ;

        // группа списков для очередей на выдачу заказов преподавателям
        // SimpleModelList<ReaderRec>[] QVZP;

        // группа списков для очередей на выдачу заказов студентам
        // SimpleModelList<ReaderRec>[] QVZS;

        // очередь свободных библиотекарей
        //  SimpleModelList<LibrarianRec> QBL;

        #endregion

        #region Cборщики статистики
        
        // 	Интенсивность числа полных циклов  ?????????????????????? 
        Variance<int> Variance_LQ;

        // МО и дисперсия количества читателей, обслуживаемых библиотекарем за один заход
        Variance<double> Variance_TQ;

        #endregion

        #region Генераторы ПСЧ

        // Генератор времени появления пассажиров
        ExpStream GenPassAppear;
        // Генератор времени прибытия автобуса на остановку
        UniformStream GenBusAppear;
        // Генератор времени высадки пассажира
        UniformStream GenPassOut;
        // Генератор времени посадки пассажира
        UniformStream GenPassIn;
        // Генератор числа выходящих пассажиров
        // DiscreteStream<int> GenKolPassOut;

        #endregion

        #region Инициализация объектов модели

        public SmoModel(Model parent, string name)
            : base(parent, name)
        {
            TQ = InitModelObject<TRealVar>();
            Variance_TQ = InitModelObject<Variance<double>>();
            Variance_TQ.ConnectOnSet(TQ);

            VQ = InitModelObject<SimpleModelList<PassRec>>();
			LQ = InitModelObject<TIntVar>();
			Variance_LQ = InitModelObject<Variance<int>>();
			Variance_LQ.ConnectOnSet(LQ);

            GenPassAppear = InitModelObject<ExpStream>("Генератор времени появления пассажиров");
            GenBusAppear = InitModelObject<UniformStream>("Генератор времени прибытия автобуса на остановку");
            GenPassOut = InitModelObject<UniformStream>("Генератор времени высадки пассажира");
            GenPassIn = InitModelObject<UniformStream>("Генератор времени посадки пассажира");
        }

        #endregion
    }
}
