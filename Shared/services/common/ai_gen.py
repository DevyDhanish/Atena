from typing import Any
import g4f
from threading import Thread
from services.pigeon.pigeon import get_pigeon_instance, DisplayType_CHAT_UI, DataType_AI_GEN_TEXT
from loguru import logger

# while True:
#     prompt = input("> ")
    # response = g4f.ChatCompletion.create(
    #     model="",
    #     provider=g4f.Provider.LambdaChat,
    #     messages=[{"role": "user", "content": f"{prompt}"}],
    #     stream=True
    # )

#     for message in response:
#         print(message, flush=True, end='')

class TextGen():
    _instance = None

    def __new__(cls, *args: Any, **kwds: Any):
        if cls._instance is not None:
            return cls._instance
        
        cls._instance = super().__new__(cls)
        return cls._instance

    def __init__(self) -> None:
        self.model = g4f.ChatCompletion()
        self.provider = g4f.Provider.LambdaChat

    def process_text(self, text):
        logger.info(text)

        get_pigeon_instance().send_data(
            DisplayType_CHAT_UI,
            DataType_AI_GEN_TEXT,
            bytearray(text, encoding="utf-8")
        )

    def gen_response(self, user_text, system_text):
        response = self.model.create(
        model="",
        provider=self.provider,
        messages=[
            {"role": "user", "content": f"{user_text}"},
            {"role" : "system", "content" : f"{system_text}"}
            ],
        stream=False
        )

        self.process_text(response)

    def generate_response(self, user_text):
        system_text = "Respond with detailed, structured, and professional explanations as if you are answering in a formal interview setting. Maintain a clear and concise style, using complete sentences and proper grammar. Avoid using emojis, special symbols, or any characters that may cause issues when transmitting over TCP"

        Thread(target=self.gen_response, args=(user_text, system_text)).start()

        return
    

# ld = TextGen()
# ld.generate_response("hey")

# while True:
#     pass