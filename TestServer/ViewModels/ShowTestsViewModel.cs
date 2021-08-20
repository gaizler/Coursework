using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALTest;
using PostSharp.Patterns.Model;
using Repository;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    class ShowTestsViewModel
    {
        [SafeForDependencyAnalysis]
        public IList TestsList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Test>();
                return repo.GetAll().Select(x => new { Title = x.Title, Time = x.Time, Author = x.Author, NumberOfAnswers = x.NumberOfQuestions, Filename = x.Filename }).ToList();
            }
        }

    }
}
