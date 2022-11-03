using System;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly CP25Team03Entities context = new CP25Team03Entities();
        private CurriculumClassRepository curriculumClassRepository;
        private CurriculumRepository curriculumRepository;
        private MajorRepository majorRepository;
        private TermRepository termRepository;
        private UserRepository userRepository;

        public CurriculumClassRepository CurriculumClassRepository
        {
            get
            {
                if (this.curriculumClassRepository == null)
                {
                    this.curriculumClassRepository = new CurriculumClassRepository(context);
                }
                return curriculumClassRepository;
            }
        }

        public CurriculumRepository CurriculumRepository
        {
            get
            {
                if (this.curriculumRepository == null)
                {
                    this.curriculumRepository = new CurriculumRepository(context);
                }
                return curriculumRepository;
            }
        }

        public MajorRepository MajorRepository
        {
            get
            {
                if (this.majorRepository == null)
                {
                    this.majorRepository = new MajorRepository(context);
                }
                return majorRepository;
            }
        }

        public TermRepository TermRepository
        {
            get
            {
                if (this.termRepository == null)
                {
                    this.termRepository = new TermRepository(context);
                }
                return termRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(context);
                }
                return userRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}