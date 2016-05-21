// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#douqbc
Blockly.Blocks['motor_reverse'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("reverse motor")
        .appendField(new Blockly.FieldDropdown([["A", "A"], ["B", "B"], ["C", "C"]]), "POSITION");
    this.setInputsInline(false);
    this.setPreviousStatement(true);
    this.setNextStatement(true);
    this.setColour(65);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#4wp22e
Blockly.Blocks['motor_set_speed'] = {
  init: function() {
    this.appendValueInput("SPEED")
        .setCheck("Number")
        .appendField("set motor")
        .appendField(new Blockly.FieldDropdown([["A", "A"], ["B", "B"], ["C", "C"]]), "POSITION")
        .appendField("speed to");
    this.setInputsInline(false);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(65);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};



// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#n6jycc
Blockly.Blocks['sensor_gyro_event'] = {
  init: function() {
    this.appendValueInput("PARAM")
        .setCheck("Number")
        .appendField("when")
        .appendField(new Blockly.FieldDropdown([["accelerometer", "ACCEL"], ["gyroscope", "GYRO"]]), "COMP")
        .appendField("axis")
        .appendField(new Blockly.FieldDropdown([["X", "X"], ["Y", "Y"], ["Z", "Z"], ["any", "ANY"]]), "AXIS")
        .appendField("exceeds");
    this.appendStatementInput("EVENT")
        .setCheck(null);
    this.setInputsInline(true);
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#88ntj5
Blockly.Blocks['sensor_temperature_event'] = {
  init: function() {
    this.appendValueInput("PARAM")
        .setCheck("Number")
        .appendField("when temperature")
        .appendField(new Blockly.FieldDropdown([[">", "GT"], [">=", "GE"], ["=", "EQ"], ["<=", "LE"], ["<", "LT"], ["!=", "NE"]]), "COMP");
    this.appendStatementInput("EVENT")
        .setCheck(null);
    this.setInputsInline(true);
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#u86f22
Blockly.Blocks['sensor_temperature_event_range'] = {
  init: function() {
    this.appendValueInput("PARAM_MIN")
        .setCheck("Number")
        .appendField("when temperature between");
    this.appendValueInput("PARAM_MAX")
        .setCheck("Number")
        .appendField("and");
    this.appendStatementInput("EVENT")
        .setCheck(null);
    this.setInputsInline(true);
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

//https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#f6t9et
Blockly.Blocks['sensor_temperature'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("temperature");
    this.setOutput(true, "Number");
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#qb3fq6
Blockly.Blocks['sensor_touch_event'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("when touch sensor")
        .appendField(new Blockly.FieldDropdown([["A", "A"], ["B", "B"], ["C", "C"]]), "SENSOR")
        .appendField("is")
        .appendField(new Blockly.FieldDropdown([["touched", "TOUCH"], ["released", "RELEASE"], ["clicked", "CLICK"]]), "STATE");
    this.appendStatementInput("EVENT")
        .setCheck(null);
    this.setInputsInline(true);
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#o5uum5
Blockly.Blocks['sensor_touch'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("touch sensor")
        .appendField(new Blockly.FieldDropdown([["A", "A"], ["B", "B"], ["C", "C"]]), "SENSOR")
        .appendField("pressed");
    this.setOutput(true, "Boolean");
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#dft6n8
Blockly.Blocks['operation_wait'] = {
  init: function() {
    this.appendValueInput("WAIT")
        .setCheck("Number")
        .appendField("wait");
    this.appendDummyInput()
        .appendField("seconds");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(240);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#q86vmc
Blockly.Blocks['program_begin_event'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("when program starts");
    this.appendStatementInput("EVENT")
        .setCheck(null);
    this.setColour(290);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#gj3yw9
Blockly.Blocks['send_message'] = {
  init: function() {
    this.appendValueInput("MESSAGE")
        .setCheck("Number")
        .appendField("send message");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(240);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};

// https://blockly-demo.appspot.com/static/demos/blockfactory/index.html#zsqkih
Blockly.Blocks['sensor_gyro_value'] = {
  init: function() {
    this.appendDummyInput()
        .appendField(new Blockly.FieldDropdown([["accelerometer", "ACCEL"], ["gyroscope", "GYRO"]]), "COMP")
        .appendField("axis")
        .appendField(new Blockly.FieldDropdown([["X", "X"], ["Y", "Y"], ["Z", "Z"]]), "AXIS");
    this.setOutput(true, "Number");
    this.setColour(165);
    this.setTooltip('');
    this.setHelpUrl('http://www.example.com/');
  }
};
