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
        private RoomRepository roomRepository;
        private TermRepository termRepository;
        private UserRepository userRepository;

        public CurriculumClassRepository CurriculumClassRepository
        {
            get
            {
                if (curriculumClassRepository == null)
                {
                    curriculumClassRepository = new CurriculumClassRepository(context);
                }
                return curriculumClassRepository;
            }
        }

        public CurriculumRepository CurriculumRepository
        {
            get
            {
                if (curriculumRepository == null)
                {
                    curriculumRepository = new CurriculumRepository(context);
                }
                return curriculumRepository;
            }
        }

        public MajorRepository MajorRepository
        {
            get
            {
                if (majorRepository == null)
                {
                    majorRepository = new MajorRepository(context);
                }
                return majorRepository;
            }
        }

        public RoomRepository RoomRepository
        {
            get
            {
                if (roomRepository == null)
                {
                    roomRepository = new RoomRepository(context);
                }
                return roomRepository;
            }
        }

        public TermRepository TermRepository
        {
            get
            {
                if (termRepository == null)
                {
                    termRepository = new TermRepository(context);
                }
                return termRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(context);
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
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}