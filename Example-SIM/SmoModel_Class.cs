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
        Variance<double>[] Variance_INTC;

        // МО и дисперсия количества читателей, обслуживаемых библиотекарем за один заход
        // Variance<double>[] Variance_KZS;

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
        DiscreteStream<int> GenKolPassOut;

        #endregion

        #region Инициализация объектов модели

        public SmoModel(Model parent, string name)
            : base(parent, name)
        {
           // TNS = InitModelObjectArray<TRealVar>(QUEUE, "время нахождения читателя в системе");
          //  KZS = InitModelObjectArray<TRealVar>(KBL, "количество читателей, обслуживаемых библиотекарем за один заход");

            VQ = InitModelObject<SimpleModelList<PassRec>>();
            GenPassAppear = InitModelObject<ExpStream>("Генератор времени появления пассажиров");
            GenBusAppear = InitModelObject<UniformStream>("Генератор времени прибытия автобуса на остановку");
            GenPassOut = InitModelObject<UniformStream>("Генератор времени высадки пассажира");
            GenPassIn = InitModelObject<UniformStream>("Генератор времени посадки пассажира");
            GenKolPassOut = InitModelObject<DiscreteStream<int>>("Генератор количества выходящих пассажиров");
            //  QVZS = InitModelObjectArray<SimpleModelList<ReaderRec>>(KBL, "очередь на выдачу заказов студентам");
            //   QBL = InitModelObject<SimpleModelList<LibrarianRec>>("очередь свободных библиотекарей");

            //??????????????????????   Variance_INTC = InitModelObjectArray<Variance<double>>(KZ , "•	Интенсивность числа полных циклов");
            // Variance_KZS = InitModelObjectArray<Variance<double>>(KBL, "МО и дисперсия KZS[]");
            //  Stud_vhod_Puason_Generator = InitModelObject<PoissonStream>("генератор потока 'время между соседними читателями в потоке студентов'");
            //  BL_Perem_Uniform_Generator = InitModelObject<UniformStream>("генератор потока 'время перемещения библиотекаря в один конец от стола заказов до хранилища'");
            //   BL_Vydacha_Uniform_Generator = InitModelObject<UniformStream>("генератор потока 'время завершения процедуры выдачи книг'");
            //   BL_poisk_Normal_Generator = InitModelObject<NormalStream>("генератор потока 'время поиска книг'");
            /*
               for (int i = 0; i <KZ; i++)
               {
                   Variance_INTC[i].ConnectOnSet(TNS[i]);
               }

               for (int i = 0; i < KBL; i++)
               {
                   Variance_KZS[i].ConnectOnSet(KZS[i]);
               }
               */
        }

        #endregion
    }
}
