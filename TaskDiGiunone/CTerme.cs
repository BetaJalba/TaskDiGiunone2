using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TaskDiGiunone
{
    public class CTerme
    {
        int n;
        int turno;
        int count;
        SemaphoreSlim semaforoPazienti;
        SemaphoreSlim semaforoCount;
        SemaphoreSlim semaforoDim;

        public CTerme(int n) 
        {
            this.n = n;
            turno = 0;
            count = 0;
            semaforoPazienti = new SemaphoreSlim(n, n);
            semaforoCount = new SemaphoreSlim(1, 1);
            semaforoDim = new SemaphoreSlim(1, 1);
        }

        async Task<bool> CanGo(bool orto)
        {
            if ((orto && turno == 2) || (!orto && turno == 1))
                return false;
            else
                return true;
        }

        async Task Attendi(bool orto)
        {
            // Attende il proprio turno
            bool canGo;

            do
            {
                await semaforoPazienti.WaitAsync();

                canGo = CanGo(orto).GetAwaiter().GetResult();

                if (!canGo)
                    semaforoPazienti.Release();
            } while (!canGo);
        }

        public async Task AccediPiscina(bool orto)
        {
            Random rand = new Random();

            await Attendi(orto);

            // Solo uno alla volta può entrare qui
            await semaforoCount.WaitAsync();

            // Se libero
            if (turno == 0)
            {
                if (orto)
                {
                    turno = 1;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nOrtopedico inizia!");
                }
                else
                {
                    turno = 2;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInfettivo inizia!");
                }
            }

            semaforoCount.Release();

            if (!CanGo(orto).GetAwaiter().GetResult())
            {
                semaforoPazienti.Release();
                await AccediPiscina(orto);
                return;
            }

            if (orto)
                Console.WriteLine("Ortopedico entra!");
            else
                Console.WriteLine("Infettivo entra!");

            count++; // Aggiunge iuser dalla psicina

            await Task.Delay(rand.Next(5));

            await semaforoDim.WaitAsync();

            int copy = --count; // Libera iuser dalla psicina

            Console.WriteLine($"Qualcuno esce! Rimasti: {copy}");

            if (count == 0) 
            {
                turno = 0;
            }

            semaforoDim.Release();
              
            semaforoPazienti.Release();
        }
    }
}