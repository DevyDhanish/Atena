import json
from typing_extensions import Self
from loguru import logger

class Config:
    _instance = None
    config_data = None
    def __new__(cls, *args, **kwargs) -> Self:
        if cls._instance is not None:
            logger.info("Using the old instance")
            return cls._instance
        cls._instance = super().__new__(cls)
        return cls._instance

    def read_config_file(self, file_path):
        with open(file_path, "r") as f_config:
            
            try:
                data = json.load(f_config)
                self.config_data = data
            except FileNotFoundError:
                logger.error(f"File {file_path} was not found")
                return
            except json.JSONDecodeError:
                logger.error(f"Unable to decode json file {file_path}")
                return
            
    def get_grpc_port(self) -> str:
        if self.config_data is not None:
            return self.config_data["port"]
        else: return ""
        
    def get_grpc_server(self) -> str:
        if self.config_data is not None:
            return self.config_data["serverAddr"]
        else: return ""
        
    def get_nest_addr(self) -> str:
        if self.config_data is not None:
            return self.config_data["serverAddr"]
        else: return ""

    def get_nest_port(self) -> str:
        if self.config_data is not None:
            return self.config_data["tcpPort"]
        else: return ""

    def get_device_name(self) -> str:
        if self.config_data is not None:
            return self.config_data["rttDevice"]
        else: return "" 
        
    def get_model_name(self) -> str:
        if self.config_data is not None:
            return self.config_data["rttModel"]
        else: return "" 