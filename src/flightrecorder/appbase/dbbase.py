class AppBaseRepository:
    """Base Repository

    Superclass for entity-specific repository implementations, providing the following features:

    1. Existence check based on a column/value pair
    2. Creation of new records
    3. Retrieval of the first/all records based on a column/value pair
    4. Retrieval of all records based on a column/value pair with a LIKE syntax

    Functions that may return more than one record return a generator.

    The repository is written for SQLAlchemy"""

    def __init__(self, session, model):
        """Repository initialisation

        :param session: SQLAlchemy session
        :param model: The SQLAlchemy model class providing table and column mappings
        """
        self._session = session
        self._model = model

    def query_exists(self, column, value):
        """Check for existence of a record

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: True if a matching record is found, False if not
        """
        record = self._session.query(self._model).filter(getattr(self._model, column) == value).first()
        return record is not None

    def insert(self, record):
        """Create a new database record

        :param record: The record to add (an instance of a SQLAlchemy Model class)
        :return: The created record
        """
        self._session.add(record)
        self._session.commit()
        return record

    def query_first(self, column, value):
        """Return the first matching record

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: The first matching record, or None for no matches
        """
        return self._session.query(self._model).filter(getattr(self._model, column) == value).first()

    def query_all(self, column, value, order_by):
        """Query all matching records (Exact Match)

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :param order_by: The name of the column to order by
        :return: A generator that will yield all matches
        """
        if column is not None and value is not None:
            for record in self._session.query(self._model).filter(getattr(self._model, column) == value)\
                    .order_by(getattr(self._model, order_by)).all():
                yield record
        else:
            for record in self._session.query(self._model).order_by(getattr(self._model, order_by)).all():
                yield record

    def query_like(self, column, value, order_by):
        """Query all matching records (LIKE)

        :param column: The name of the column to search
        :param value: The value to look for (LIKE substring match)
        :param order_by: The name of the column to order by
        :return: A generator that will yield all matches
        """
        pattern = "%{}%".format(value)
        for record in self._session.query(self._model).filter(getattr(self._model, column).like(pattern))\
                .order_by(getattr(self._model, order_by)).all():
            yield record
