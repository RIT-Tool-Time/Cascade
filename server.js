var app = require('http').createServer(handler);
var io = require('socket.io').listen(app);
var fs = require('fs');
var tcp = null;
var array = [];

// Which port number and IP address we're listening on
app.listen(8000, '129.21.67.6');

function handler (req, res) {
  	fs.readFile(__dirname + '/index.html',
  	function (err, data) {
    	if (err) {
      		res.writeHead(500);
      		return res.end('Error loading index.html');
    	}

    	res.writeHead(200, {'Content-Type': 'text/html'});
    	res.end(data);
  	});
}

/*
	Stuff to communicate between JS and C#
*/

io.sockets.on('connection', function (socket) {	
	
	array.push(socket);
	
	// Opening message when a connection is made
	io.sockets.emit('this', { will: 'be received by everyone'});
	
	// Sending the player's name and number
	socket.on('name', function(p, n, e, c) {
	  	console.log('n:' + p + ':' + n + ':' + e + ':' + c + ';');	
	  	
	  	if (tcp != null) {
			   	
	    	tcp.write('n:' + p + ':' + n + ':' + e + ':' + c + ';');
    	}
  	});
	
	// Sending the touch to tcp
  	socket.on('touch', function (p, x, y, id, finger) {
    	
	   	console.log('t:' + p + ':' + x.toString() + ',' + y.toString() + ':' + id + ':' + finger + ';');

    	if (tcp != null) {
			
			// Convert the number to a string    	
	    	tcp.write('t:' + p + ':' +  x.toString() + ',' + y.toString() + ':' + id + ':' + finger + ';');
    	}
  	});
  	
  	// Sending the touch's x position and its ID to tcp
  	socket.on('move', function (p, x, y, id, finger) {
    
    	console.log('m:' + p + ':' + x.toString() + ',' + y.toString() + ':' + id + ':' + finger + ';');
    
    	if (tcp != null) {
			
			// Convert the number to a string    	
	    	tcp.write('m:' + p + ':' + x.toString() + ',' + y.toString() + ':' + id + ':' + finger + ';');
    	}
  	});
  	
  	// We need to check when a touch is removed from the display
  	socket.on('cancel', function(id) {
	  	
	  	console.log('c: ' + id + ';');
	  	
	  	if(tcp != null) {
	  		
	  		// Send a message to tcp
		  	tcp.write('c:' + id + ';');
	  	}
  	});

  	// Disconnecting
  	socket.on('disconnect', function () {
  	
  		// Message to write when user disconnects
    	io.sockets.emit('user disconnected');
    	
    	array = [];
  	});
});

var net = require('net');
var server = net.createServer(function(c) { 
	
	// Setting parameter to global variable to use on both ports
	tcp = c;
	
	tcp.on('data', function(e){
		
		// Logging the message
		console.log(e.toString());
		
		for(var i=0; i<array.length; i++) {
			array[i].emit('game-over', e.toString());
		}
	});
	
  	// Sending the information from the events to C# program listening on a different port 
	c.pipe(c);
});

server.listen(8124, function() { 

	// Listener for Chris to connect through C#
	console.log('server bound');
});
