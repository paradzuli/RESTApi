using DataModel.GenericRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;


namespace DataModel.UnitOfWork
{
   public class UnitOfWork:IDisposable, IUnitOfWork
    {
        #region Private variables

       private WebApiDbEntities _context = null;
       private GenericRepository<User> _userRepository;
       private GenericRepository<Product> _productRepository;
       private GenericRepository<Token> _tokenRepository;

       #endregion

       public UnitOfWork()
       {
           _context = new WebApiDbEntities();
       }

        #region Public Properties

       public GenericRepository<Product> ProductRepository
       {
           get
           {
               if(_productRepository==null)
                    _productRepository = new GenericRepository<Product>(_context);
               return _productRepository;
           }
       }

       public GenericRepository<User> UserRepository
       {
           get
           {
               if (_userRepository == null)
               {
                   _userRepository = new GenericRepository<User>(_context);
               }
               return _userRepository;
               
           }
       }

       public GenericRepository<Token> TokenRepository
       {
           get
           {
               if (_tokenRepository == null)
               {
                   _tokenRepository = new GenericRepository<Token>(_context);
               }
               return _tokenRepository;
           }
       }


       public void Save()
       {
           try
           {
               _context.SaveChanges();
           }
           catch (DbEntityValidationException e)
           {
               var outputLines = new List<string>();
               foreach (DbEntityValidationResult error in e.EntityValidationErrors)
               {
                   outputLines.Add(String.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now,error.Entry.State));
                   foreach (var ve in error.ValidationErrors)
                   {
                       outputLines.Add(String.Format("- Property: \"{0}\", Error:\"{1}\"",ve.PropertyName,ve.ErrorMessage));
                   }
               }
               System.IO.File.AppendAllLines(@"C:\webapierrors.txt",outputLines);
           }
       }

        #endregion

        #region Implementation of IDisposable

       private bool disposed = false;

       protected virtual void Dispose(bool disposing)
       {
           if (!disposed)
           {
               if (disposing)
               {
                   Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
               }
           }
           disposed = true;
       }

       public void Dispose()
       {
           Dispose(true);
            GC.SuppressFinalize(this);
       }

       #endregion
    }
}
