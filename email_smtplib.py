import argparse
import smtplib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
import keyring
import getpass
import socket


class MailService:  # Office 365
    def __init__(self, user, psw):
        self.user = user
        self.psw = psw
        # set up the SMTP server
        self.s = smtplib.SMTP(host='smtp-mail.outlook.com', port=587)
        self.s.starttls()
        self.s.login(self.user, self.psw)

    def __del__(self):
        try:
            self.s.quit()
        except:
            pass

    def send(self, to, subject, body):
        msg = MIMEMultipart()  # create a message
        msg['From'] = self.user
        msg['To'] = to
        msg['Subject'] = subject
        msg.attach(MIMEText(body))
        self.s.send_message(msg)
        del msg


def get_ip():
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.connect(("8.8.8.8", 80))
    sock_name = s.getsockname()[0]
    s.close()
    return sock_name


def clear_stored():
    print('Clearing stored passwords')
    try:
        keyring.delete_password("system", "user")
    except:
        pass
    try:
        keyring.delete_password("system", "password")
    except:
        pass
    try:
        keyring.delete_password("system", "to")
    except:
        pass
    try:
        keyring.delete_password("system", "ip")
    except:
        pass


parser = argparse.ArgumentParser()
parser.add_argument('--reset', help='reset wipes stored password', action='store_true')
args = parser.parse_args()
if args.reset:
    clear_stored()
    exit(0)


print('Starting...')
my_address = keyring.get_password("system", "user")
password = keyring.get_password("system", "password")
to_address = keyring.get_password("system", "to")
if password is None or to_address is None or password is None:
    print("Office 365 login")
    print('Login e-mail: ', end='')
    my_address = input()
    password = getpass.getpass("Password: ")
    print('To e-mail: ', end='')
    to_address = input()
    try:
        m = MailService(my_address, password)
        keyring.set_password("system", "user", my_address)
        keyring.set_password("system", "password", password)
        keyring.set_password("system", "to", to_address)
    except:
        exit(-1)

ip_address = keyring.get_password("system", "ip")
if ip_address is None:
    ip_address = get_ip()
else:
    new_ip_address = get_ip()
    if ip_address == ip_address:
        print(f'IP unchanged {ip_address}')
        exit(0)

keyring.set_password("system", "ip", ip_address)
print(f'IP {ip_address} being mailed to {to_address}')

try:
    m = MailService(my_address, password)
except:
    clear_stored()
    exit(-1)

m.send(to_address, 'IP update', f'New IP {ip_address}')
del m
