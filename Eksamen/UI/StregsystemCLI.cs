﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Schema;
using Eksamen;
using Eksamen.Core;

namespace Eksamen.UI
{
    public class StregsystemCLI : IStregsystemUI
    {
        private IStregsystem IS;
        private bool _running = true;
        public StregsystemCLI(IStregsystem s)
        {
            IS = s;
            IS.FileReadError += DisplayFileReadError;
            IS.UserBalanceWarning += UserBalanceWarning;
            IS.ReadFiles();
        }

        private void DisplayFileReadError(string message)
        {
            _running = false;
            Console.WriteLine($"Files could not be read because of this invalid data: ({message})");
            Console.WriteLine($"Closing application...");
        }

        public void DisplayUserNotFound(string username)
        {
            Console.WriteLine($"User with: ({username}) as a username could not be found");
        }

        public void DisplayProductNotFound(string product)
        {
            Console.WriteLine($"Product with: ({product}) could not be found");
        }

        public void DisplayUserInfo(User user)
        {
            List<Transaction> transactions = (List<Transaction>)IS.GetTransactions(user, 10);
            Console.WriteLine($"{user}: Balance: {user.Balance}");
            Console.WriteLine($"Last {transactions.Count} transations:");
            foreach (Transaction itemTransaction in transactions)
            {
                Console.WriteLine(itemTransaction);
            }

            Console.WriteLine("");
        }

        public void DisplayTooManyArgumentsError(string command)
        {
            Console.WriteLine($"Too many arguments were entered: ({command})");
        }

        public void DisplayAdminCommandNotFoundMessage(string adminCommand)
        {
            Console.WriteLine($"This admin command: ({adminCommand}) was not found");
        }

        public void DisplayUserBuysProduct(BuyTransaction transaction)
        {
            Console.WriteLine(transaction);
        }

        public void DisplayUserBuysProduct(int count, BuyTransaction transaction)
        {
            Console.WriteLine($"{count}x {transaction}");
        }

        public void Close()
        {
            Console.WriteLine($"Closing program, bye!");
            _running = false;
        }

        public void DisplayInsufficientCash(User user, Product product)
        {
            Console.WriteLine($"User: {user.Username} (balance: {user.Balance}) did not have sufficient funds to purchase: {product.Name}");
        }

        public void DisplayGeneralError(string errorString)
        {
            Console.WriteLine($"Error: {errorString}");
        }

        public void DisplayUserTransactions(List<Transaction> transactions)
        {
            foreach (Transaction itemTransaction in transactions)
            {
                Console.WriteLine(itemTransaction);
            }
        }

        private void UserBalanceWarning(User user, decimal balance)
        {
            Console.WriteLine($"WARNING! User: {user.Username}'s balance is only: {balance}");
        }

        public void Start()
        {

            while (_running)
            {
                WriteMenu();

                try
                {
                    HandleInput();
                }
                catch (NonExistingUserException e) //TODO GØR DET HER MINDRE KOKS
                {
                    DisplayGeneralError(e.Message);
                }
                catch (NonExistingProductException e)
                {
                    DisplayGeneralError(e.Message);
                }
                catch (InsufficientCreditsException e)
                {
                    DisplayGeneralError(e.Message);
                }
                catch (NotActiveProductException e)
                {
                    DisplayGeneralError(e.Message);
                }
            }
        }

        public event StregsystemEvent CommandEntered;

        public void HandleInput()
        {
            string command = Console.ReadLine();
            CommandEntered?.Invoke(command);
        }

        public void WriteMenu()
        {
            foreach (Product isActiveProduct in IS.ActiveProducts)
            {
                Console.WriteLine(isActiveProduct);
            }
            Console.WriteLine("");
            Console.Write("Enter command: ");

        }

    }
}