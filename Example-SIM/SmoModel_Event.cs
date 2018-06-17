using System;
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
            public Pass ZP;
            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                Model.KVZ += 1;
                Model.Tracer.EventTrace(this, "NZ = " + ZP.NZ);
                var rec = new PassRec();                            //новая запись 
                rec.Z = ZP;                                         //передаём в созданный объект, объект заявки
                Model.VQ.Add(rec);                                  //добавляем в очередь
				Model.LQ.Value = Model.VQ.Count.Value;

                Model.times.Add(Time);
                Model.iPassIn++;

                var ev1 = new K1();                                 // создание объекта события
                ZP.NZ += 1;
                ev1.ZP = ZP;                                        // передача библиотекаря в событие
                double dt1 = Model.GenPassAppear.GenerateValue();
                Model.PlanEvent(ev1, dt1);                          // планирование события 3
                Model.Tracer.PlanEventTrace(ev1);
                Model.Tracer.AnyTrace("");
                Model.TraceModel();
                Model.Tracer.AnyTrace("");
            }
        }

        // класс для события 2 - приход автобуса на остановку
        public class K2 : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события
			public Bus ZB;
            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                
				Random rnd = new Random();
                Model.KPA = rnd.Next(Model.ml, Model.mp);
                Model.KA += 1;
                Model.Tracer.EventTrace(this, "NB = " + ZB.NB + " KPA = " + Model.KPA);
                Model.SA = 1;
				int KolPassOut = rnd.Next(Model.VL, Model.VP);
                
                //           if (KolPassOut > ZB.KPA)
                //KolPassOut = ZB.KPA;

                Model.KOP = KolPassOut;
              // /* Model.KOP = */Model.KPA -= KolPassOut;

                // ВЫСАДКА ПАССАЖИРОВ
				double dt3 = 0;
				for (int i = 0; i <= KolPassOut; i++)
					dt3 += Model.GenPassOut.GenerateValue();

                var ev3 = new K3();                                 // создание объекта события
                Model.PlanEvent(ev3, dt3);                          // планирование события 3
                Model.Tracer.PlanEventTrace(ev3);
                
				var ev2 = new K2();                                 // создание объекта события
                Bus Z = new Bus();
                Z.NB = this.ZB.NB + 1;
                Z.KPA = Model.KPA;
                ev2.ZB = Z;
				double dt2 = Model.T + Model.GenBusAppear.GenerateValue();
                Model.PlanEvent(ev2, dt2);                          // планирование события 3
                Model.Tracer.PlanEventTrace(ev2);
                Model.Tracer.AnyTrace("");
                Model.TraceModel();
                Model.Tracer.AnyTrace("");
            }
        }

        // класс для события 3 - конец высадки пассажиров
        public class K3 : TimeModelEvent<SmoModel>
        {
			// алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                Model.KPA -= Model.KOP;
                Model.Tracer.EventTrace(this, "KPA = " + Model.KPA);

                if (Model.VQ.Count != 0)
        		{
        			Model.SA = 2;
        			Model.VQ.RemoveAt(0);
                    Model.iPassOut++;
                    Model.times[Model.iPassOut] = Time - Model.times[Model.iPassOut];

                    Model.KPA += 1;
					Model.LQ.Value -= 1;
        			double dt4 = Model.GenPassIn.GenerateValue();
        			var ev4 = new K4();                                 // создание объекта события
                    Model.PlanEvent(ev4, dt4);                          // планирование события 3
					Model.Tracer.PlanEventTrace(ev4);
        		}
        		else
        		{
        			Model.SA = 0;
        		}

				Model.Tracer.AnyTrace("");
                Model.TraceModel();
                Model.Tracer.AnyTrace("");
            }
        }
        
		// класс для события 4 - посадка в автобус
        public class K4 : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события
            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
				Model.Tracer.EventTrace(this);

                if (Model.VQ.Count.Value != 0)
                {
					if (Model.B - Model.KPA > 0)
					{
						Model.SA = 2;
						Model.VQ.RemoveAt(0);
                        Model.iPassOut++;
                        Model.times[Model.iPassOut] = Time - Model.times[Model.iPassOut];
                        Model.LQ.Value -= 1;
						Model.KPA += 1;
						double dt4 = Model.GenPassIn.GenerateValue();
						var ev4 = new K4();                                 // создание объекта события
						Model.PlanEvent(ev4, dt4);                          // планирование события 3
                        Model.Tracer.PlanEventTrace(ev4);
                    }
					else 
					{
						Model.SA = 0;
						if (Model.NVAR == 2)
						{
                            while (Model.times.Count != Model.iPassOut)
                            {
                                Model.iPassOut++;
                                Model.times[Model.iPassOut] = Time - Model.times[Model.iPassOut];
                            }
                            Model.KNP += Model.VQ.Count;
							Model.VQ.Clear();
						}
					}
                }
                else
                {
                    Model.SA = 0;
                }

                Model.Tracer.AnyTrace("");
                Model.TraceModel();
                Model.Tracer.AnyTrace("");
            }
        }
    }
}
