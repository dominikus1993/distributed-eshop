

import datetime
from typing import Coroutine
from pymongo.collection import Collection

from pymongo.database import Database
from core.data import CustomerShoppingList, CustomerShoppingListHistoryWriter
import pymongo

class MongoCustomerShoppingListHistoryWriter(CustomerShoppingListHistoryWriter):
    __client: pymongo.MongoClient
    __collection: Collection
    __db: Database
    def __init__(self, c: pymongo.MongoClient) -> None:
        self.__client = c
        self.__db = c.get_database("History")
        self.__collection = self.__db.get_collection("shopping_list_log")

    async def store_changes(self, evt: CustomerShoppingList):
        items = []
        for item in evt.items:
            items.append({"item_id": item.item_id, "item_quantity": item.item_quantity});
        self.__collection.insert_one({ "customer_id": evt.customer_id, "items": items, "changed_at": datetime.datetime.utcnow(), "event_type": "CustomerShoppingListChanged" })

    async def store_deletion(self, customer_id: int) :
        items = []
        self.__collection.insert_one({ "customer_id": customer_id, "removed_at": datetime.datetime.utcnow(), "event_type": "CustomerShoppingListRemoved" })