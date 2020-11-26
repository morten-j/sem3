﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Eksamen.Core;
using Eksamen.UI;

namespace Eksamen.Controller
{
    public class StregsystemController
    {
        private IStregsystem S;
        private IStregsystemUI SUI;
        private Dictionary<string, Action<string[]>> _adminCommands = new Dictionary<string, Action<string[]>>()
        {
            //{ ":quit", (string[] command) => SUI.Close() }, //TODO KAN MAN RYKKE DET HEROP???
            //{ ":q", (string[] command) => SUI.Close() },
        };

        public StregsystemController(IStregsystemUI sui, IStregsystem s)
        {
            S = s;
            SUI = sui;
            SUI.CommandEntered += ParseCommand;
            _adminCommands.Add(":quit", (string[] command) => SUI.Close());
            _adminCommands.Add(":q", (string[] command) => SUI.Close());
            _adminCommands.Add(":activate", (string[] command) => S.GetProductByID(Convert.ToInt32(command[1])).Active = true);
            _adminCommands.Add(":deactivate", (string[] command) => S.GetProductByID(Convert.ToInt32(command[1])).Active = false);
            _adminCommands.Add(":crediton", (string[] command) => S.GetProductByID(Convert.ToInt32(command[1])).CanBeBoughtOnCredit = true);
            _adminCommands.Add(":creditoff", (string[] command) => S.GetProductByID(Convert.ToInt32(command[1])).CanBeBoughtOnCredit = false);
            _adminCommands.Add(":addcredits", (string[] command) => S.AddCreditsToAccount(s.GetUserByUsername(command[1]), Convert.ToInt32(command[2])));
        }
        //TODO MÅSKE HÅNTERE EXCEPTIONS HERINDE LIGESOM ALLE ANDRE PÅ DISCORD
        public void ParseCommand(string command)
        {
            User user;
            Product product;
            if (command.StartsWith(":"))
            {
                string[] adminSplit = command.Split(' ');

                foreach (KeyValuePair<string, Action<string[]>> commandAction in _adminCommands)
                {
                    if (adminSplit[0] == commandAction.Key)
                    {
                        commandAction.Value?.Invoke((adminSplit));
                        break;
                    }

                    if (commandAction.Equals(_adminCommands.Last()))
                    {
                        SUI.DisplayAdminCommandNotFoundMessage($"{adminSplit[0]}");
                    }

                }

            }
            else
            { //TODO MÅSKE EN STOR  TRY CATCH????
                string[] split = command.Split(' ');
                user = TestUser(split[0]);
                if (split.Length == 3) //QUICKBUY
                {
                    product = TestProduct(split[1]);
                    MultiBuyProduct(user, product, Convert.ToInt32(split[2]));
                }
                else if (split.Length == 2) //Normal
                {
                    product = TestProduct(split[1]);
                    BuyProduct(user, product);
                }
                else if (split.Length == 1)
                {
                    TypedInUsername(user);
                }
                else if (split.Length > 3)
                {
                    SUI.DisplayTooManyArgumentsError(command);
                }

            }

        }

        private User TestUser(string split)
        {
            User user;
            try
            {
                user = S.GetUserByUsername(split);
            }
            catch (NonExistingUserException e)
            {
                SUI.DisplayUserNotFound(split);
                throw;
            }

            return user;
        }

        private Product TestProduct(string productID)
        {
            Product product;
            try
            {
                product = S.GetProductByID(Convert.ToInt32(productID));
            }
            catch (NonExistingProductException e)
            {
                SUI.DisplayProductNotFound(productID);
                throw;
            }

            return product;
        }

        private void TypedInUsername(User user)
        {
            SUI.DisplayUserInfo(user);
        }

        private void BuyProduct(User user, Product product)
        {
            SUI.DisplayUserBuysProduct(S.BuyProduct(user, product));
        }

        private void MultiBuyProduct(User user, Product product, int count)
        {
            BuyTransaction bt = null;
            for (int i = 0; i < count; i++)
            {
                bt = S.BuyProduct(user, product);
            }
            SUI.DisplayUserBuysProduct(count, bt);
        }

    }
}