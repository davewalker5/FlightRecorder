from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbmanufacturerrepo import ManufacturerRepository
from flightrecorder.db.dbmodels import ModelModel
from flightrecorder.objectmodel.ommodel import Model


class ModelRepository(AppBaseRepository):
    """Repository for addition and querying of aircraft models"""

    def __init__(self, session):
        super(ModelRepository, self).__init__(session, ModelModel)

    def read(self, column, value):
        """Load and return a specified model

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: A Model object instance for the matching model, or None if there is no match
        """
        model = super(ModelRepository, self).query_first(column, value)
        if model is None:
            return None
        m = ManufacturerRepository(self._session)
        manufacturer = m.read("id", model.manufacturer_id)
        return Model(model.name, manufacturer, model.id)

    def read_all_for_manufacturer(self, manufacturer_name):
        """Return all aircraft models associated with a specified manufacturer

        :param manufacturer_name: String containing the manufacturer name
        :return: A generator to yield a Model instance for each match
        """
        manufacturer_repo = ManufacturerRepository(self._session)
        if manufacturer_repo.exists(manufacturer_name):
            manufacturer = manufacturer_repo.read("name", manufacturer_name)
            for model in super(ModelRepository, self).query_all("manufacturer_id", manufacturer.db_id(), "name"):
                yield Model(model.name, manufacturer, model.id)

    def create(self, name, manufacturer):
        """Create a new model or return an existing one

        The check for an existing model uses the model name, as these are assumed to be unique for a given
        manufacturer.

        :param name: String containing the model name
        :param manufacturer: String containing the manufacturer name
        :return: A Model object instance for the model
        """
        exists = super(ModelRepository, self).query_exists("name", name)
        if not exists:
            m = ManufacturerRepository(self._session)
            manufacturer_id = m.create(manufacturer).db_id()
            super(ModelRepository, self).insert(ModelModel(manufacturer_id=manufacturer_id, name=name))
        return self.read("name", name)
