@echo off


python -m grpc_tools.protoc -Iproto\ --python_out=. --grpc_python_out=. proto\servicecmd.proto

mv *.py services/