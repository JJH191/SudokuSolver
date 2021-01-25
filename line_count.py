import os

line_count = 0
for root, directories, filenames in os.walk('./'):
    for filename in filenames:
        if filename.endswith('.cs') or filename.endswith('.xaml'):
            with open(os.path.join(root,filename), 'r') as f:
                for line in f.readlines():
                    line = line.strip()
                    if len(line) != 0 and not line.startswith('//'): # Remove comments and empty lines
                        line_count += 1

print(f'You have painfully written {line_count} lines of code. You need to stop and get some help')
input()
