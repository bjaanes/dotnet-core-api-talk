const bookSchema = require('../db/book-schema');

class BookController {

  constructor(Book) {
    this.Book = Book;
  }

  find(req, res, next) {
    return this.Book.find(req.query)
      .then(collection => {
        let view = [];
        collection.forEach(function (book) {
          view.push({
            id: book.id,
            title: book.Title,
            author: book.Author,
            isbn: book.isbn
          })
        });
        res.status(200).json(view);
      })
      .catch(err => next(err));
  }

  create(req, res, next) {
    let book = new this.Book();
    book.Title = req.body.title;
    book.Author = req.body.author;
    book.isbn = req.body.isbn;

    book.save()
      .then(doc => {
        res.status(201).json({
          id: doc.id,
          title: doc.Title,
          author: doc.Author,
          isbn: doc.isbn
        })
      })
      .catch(err => next(err));
  }

  update(req, res, next) {
    const conditions = { _id: req.params.id };

    this.Book.update(conditions, {
      Title: req.body.title,
      Author: req.body.author,
      isbn: req.body.isbn
    })
      .then(doc => {
        if (!doc) { return res.status(404).end(); }
        return res.status(200).json({
          id: doc.id,
          title: doc.Title,
          author: doc.Author,
          isbn: doc.isbn
        });
      })
      .catch(err => next(err));
  }

  remove(req, res, next) {
    this.Book.findByIdAndRemove(req.params.id)
      .then(doc => {
        if (!doc) { return res.status(404).end(); }
        return res.status(204).end();
      })
      .catch(err => next(err));
  }
}

module.exports = new BookController(bookSchema);
