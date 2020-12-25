using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace WebScraping
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://srh.bankofchina.com/search/whpj/searchen.jsp");

                var startDate = driver.FindElement(By.Name("erectDate"));
                var endDate = driver.FindElement(By.Name("nothing"));
                var currency = driver.FindElement(By.Name("pjname"));
                var selectElement = new SelectElement(currency);
                var nextPage = driver.FindElements(By.XPath("/html/body/table[3]/tbody/tr/td/div/span"));
                var searchButton = driver.FindElement(By.XPath("//input[@value='search']"));

                startDate.SendKeys("2020-12-24");
                endDate.SendKeys("2020-12-26");
                selectElement.SelectByValue("EUR");
                searchButton.Click();

                Thread.Sleep(1000);

                var nextPageBtn = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr/td/div/span[3]/a"));

                var lastElement = driver.FindElement(By.XPath("//*[contains(@title, \"Last Page\")]/a")).Text;

                int elementNum;
                bool success = Int32.TryParse(lastElement, out elementNum);


                var tableDataElementsHeader = driver.FindElements(By.XPath("/html/body/table[2]/tbody/tr[1]/td"));
                var tableDataElements = driver.FindElements(By.XPath("/html/body/table[2]/tbody/tr/td"));

                List<string> tableDataHeader = new List<string>();
                List<string> tableData = new List<string>();

                for (int i = 0; i < tableDataElementsHeader.Count; i++)
                {
                    var innerHtml = tableDataElementsHeader[i].GetAttribute("innerHTML");
                    Console.Write(innerHtml);

                    tableDataHeader.Add(innerHtml.ToString());

                }

                for (int i = 0; i < elementNum; i++)
                {
                    tableDataElements = driver.FindElements(By.XPath("/html/body/table[2]/tbody/tr/td"));

                    for (int j = 7; j < tableDataElements.Count; j++)
                    {

                        try
                        {

                            var innerHtml = tableDataElements[j].GetAttribute("innerHTML");
                            Console.Write(innerHtml);

                          
                            tableData.Add(innerHtml.ToString());

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }


                    try
                    {
                        nextPageBtn = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr/td/div/span[3]/a"));
                        tableData.Add("Strana " + i + " ---------\n");
                        nextPageBtn.Click();
                        Thread.Sleep(2000);
                    }
                    catch (StaleElementReferenceException ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                string fileLocation = @"result.txt";

                using (StreamWriter streamWriter = File.CreateText(fileLocation))
                {
                    if (fileLocation.Contains("result.txt"))
                    {
                        streamWriter.Write(String.Empty);
                        for (int i = 0; i < tableDataHeader.Count; i++)
                        {
                            streamWriter.Write(String.Format("{0}; ", tableDataHeader[i]));
                        }

                        streamWriter.Write("\n");

                        for (int i = 0; i < tableData.Count; i++)
                        {
                            streamWriter.Write(String.Format("{0}; ", tableData[i]));
                            if (tableData[i].Contains(":"))
                                streamWriter.Write("\n");

                        }
                    }
                }
                driver.Quit();
                Console.ReadKey();
            }
        }
    }
}


