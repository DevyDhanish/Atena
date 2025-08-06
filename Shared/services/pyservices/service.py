from abc import abstractmethod, ABC

OK = 0
FAIL = 1

class Service(ABC):
    @abstractmethod 
    def on_execute(self, data : bytearray) -> tuple:
        pass