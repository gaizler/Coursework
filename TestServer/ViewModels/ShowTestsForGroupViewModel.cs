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
    class ShowTestsForGroupViewModel
    {
        #region Variables
        public Group SelectedGroup { get; set; } = null;

        [SafeForDependencyAnalysis]
        public IList TestList
        {
            get
            {
                if (SelectedGroup != null)
                {
                    GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                    var repo = unitOfWork.Repository<Group>();
                    return repo.GetWithInclude(x=>x.Id==SelectedGroup.Id,t=>t.Tests).First().Tests.Select(x => new { Title = x.Title, Time = x.Time, Author = x.Author, NumberOfAnswers = x.NumberOfQuestions, Filename = x.Filename }).ToList();
                }
                return null;
            }
        }
        [SafeForDependencyAnalysis]
        public IList GroupList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Group>();
                return repo.GetAll().ToList();
            }
        }
        #endregion
    }
}
