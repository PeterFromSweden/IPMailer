import keyring
import getpass

p = keyring.get_password("system", "password")
if p is not None:
    print(p)
else:
    p = getpass.getpass()
    print(f'Dont tell: {p}')
    keyring.set_password("system", "password", p)
