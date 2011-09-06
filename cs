#!/usr/bin/expect -f
#cs: expect script to automate connection to cisco routers and switches with enable privileges.
#Usage: cs nick_router_name
#It uses expect, and according the config file defined at $HOME/.ciscorouters, permit both telnet and ssh connections.
#it could be executed with the "con" script file or just alone, depending if you want to keep thesession on the current terminal or on a new independent terminal either. 
#Jose Antonio Montes 2011

set timeout 10
set server [lindex $argv 0]

set filename [open "~/.ciscorouters" r]
set routers_list [split [read $filename] "\n"]
close $filename

foreach router $routers_list {

	if { $server == [lindex $router 0] } {

		set IP		[lindex $router 2] 
		set user	[lindex $router 3] 
		set password	[lindex $router 4] 
		set enpassword	[lindex $router 5] 
		set isssh	[lindex $router 6] 

		if { ${isssh} == "ssh" || ${isssh} == "jun" } {

		  spawn ssh ${user}@${IP}
		  expect {

			"yes/no" { 
					send "yes\r"
					exp_continue
				}

			"assword: " { send "$password\r" }

		  }

		  expect ">"

      if { ${isssh} == "ssh" } {

		    send "en\r"
		    expect "assword: "
		    send "$enpassword\r"

      }

		  interact
		  exit

		} else {

                  spawn telnet $IP
                  expect "Username: "
                  send "$user\r"
                  expect "Password: "
                  send "$password\ren\r"
                  expect "Password: "
                  send "$enpassword\r"
                  interact
                  exit

		}

	}

}

send_user "The $server doesn\'t exist into the router list\n"
