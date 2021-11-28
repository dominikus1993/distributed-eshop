

import datetime
from pymongo.collection import Collection

from pymongo.database import Database
from core.data import CustomerShoppingList, CustomerShoppingListChanged, CustomerShoppingListHistoryReader, CustomerShoppingListHistoryWriter, CustomerShoppingListRemoved, Item
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
            items.append({"item_id": item.item_id,
                         "item_quantity": item.item_quantity})
        self.__collection.insert_one({"customer_id": evt.customer_id, "items": items,
                                     "changed_at": datetime.datetime.utcnow(), "event_type": "CustomerShoppingListChanged"})

    async def store_deletion(self, customer_id: int):
        self.__collection.insert_one({"customer_id": customer_id, "removed_at": datetime.datetime.utcnow(
        ), "event_type": "CustomerShoppingListRemoved"})


class MongoCustomerShoppingListHistoryReader(CustomerShoppingListHistoryReader):
    __client: pymongo.MongoClient
    __collection: Collection
    __db: Database

    def __init__(self, c: pymongo.MongoClient) -> None:
        self.__client = c
        self.__db = c.get_database("History")
        self.__collection = self.__db.get_collection("shopping_list_log")

    async def read(self, customer_id: int):
        for log in self.__collection.find({"customer_id": customer_id}):
            if log["event_type"] == "CustomerShoppingListChanged":
                items = []
                for item in log["items"]:
                    items.append(
                        Item(item_id=item["item_id"], item_quantity=item["item_quantity"]))

                yield CustomerShoppingListChanged(log["customer_id"], items, log["changed_at"])
            else:
                yield CustomerShoppingListRemoved(log["customer_id"], log["removed_at"])
