import json
from loguru import logger


class Config:
    def __init__(self, path_to_config_file):
        self.config_data = None

        with open(path_to_config_file, "r") as f_config:
            
            try:
                data = json.load(f_config)
                self.config_data = data

            except FileNotFoundError:
                logger.error(f"File {path_to_config_file} was not found")
                return
            except json.JSONDecodeError:
                logger.error(f"Unable to decode json file {path_to_config_file}")
                return
            
    def get_nest_addr(self):
        if self.config_data is not None:
            return self.config_data["serverAddr"]

    def get_nest_port(self):
        if self.config_data is not None:
            return self.config_data["tcpPort"]
        


CONFIG_INSTANCE = None